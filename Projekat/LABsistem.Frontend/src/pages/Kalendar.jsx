import { useEffect, useMemo, useState } from "react";
import dayjs from "dayjs";
import "dayjs/locale/bs";
import { Calendar, Views, dayjsLocalizer } from "react-big-calendar";
import "react-big-calendar/lib/css/react-big-calendar.css";
import api from "../api/client";
import Layout from "../components/Layout";

dayjs.locale("bs");

const localizer = dayjsLocalizer(dayjs);

const CALENDAR_MESSAGES = {
  allDay: "Cijeli dan",
  previous: "Nazad",
  next: "Naprijed",
  today: "Danas",
  month: "Mjesec",
  week: "Sedmica",
  day: "Dan",
  agenda: "Agenda",
  date: "Datum",
  time: "Vrijeme",
  event: "Termin",
  noEventsInRange: "Nema termina u odabranom periodu.",
  showMore: (count) => `+${count} vise`,
};

const STATUS_META = {
  available: {
    label: "Slobodan",
    className: "available",
    badgeClassName: "zeleno",
    color: "#15803d",
    background: "#dcfce7",
    border: "#86efac",
  },
  occupied: {
    label: "Zauzet",
    className: "occupied",
    badgeClassName: "plavo",
    color: "#1d4ed8",
    background: "#dbeafe",
    border: "#93c5fd",
  },
  blocked: {
    label: "Blokiran",
    className: "blocked",
    badgeClassName: "crveno",
    color: "#b91c1c",
    background: "#fee2e2",
    border: "#fca5a5",
  },
};

function extractErrorMessage(error, fallbackMessage) {
  const responseData = error?.response?.data;
  if (typeof responseData === "string" && responseData.trim()) {
    return responseData;
  }
  if (
    typeof responseData?.message === "string" &&
    responseData.message.trim()
  ) {
    return responseData.message;
  }
  return fallbackMessage;
}

function toDateOnly(value) {
  if (!value) return "";
  return value.split("T")[0];
}

function formatDate(value) {
  if (!value) return "N/A";
  return dayjs(value).format("DD.MM.YYYY.");
}

function formatTime(value) {
  if (!value) return "";
  return value.slice(0, 5);
}

function toLocalDate(dateValue, timeValue) {
  const dateOnly = toDateOnly(dateValue);
  if (!dateOnly || !timeValue) return null;

  const [year, month, day] = dateOnly.split("-").map(Number);
  const [hours, minutes, seconds] = timeValue.split(":").map(Number);

  return new Date(
    year,
    (month || 1) - 1,
    day || 1,
    hours || 0,
    minutes || 0,
    seconds || 0,
    0,
  );
}

function resolveTerminStatus(termin) {
  const rawStatus = String(termin.statusTermina || "").toLowerCase();

  if (rawStatus === "otkazan" || rawStatus === "odbijen") {
    return "blocked";
  }

  if (rawStatus === "rezervisan") {
    return "occupied";
  }

  if (rawStatus === "slobodan") {
    return "available";
  }

  // Fallback na staru logiku ako nema statusTermina
  if (rawStatus.includes("blok")) return "blocked";
  if (rawStatus.includes("zauzet") || rawStatus.includes("rezervisan")) return "occupied";

  return "available";
}

function getDefaultView() {
  if (typeof window !== "undefined" && window.innerWidth < 900) {
    return Views.AGENDA;
  }

  return Views.MONTH;
}

function Kalendar() {
  const [termini, setTermini] = useState([]);
  const [selectedKabinetId, setSelectedKabinetId] = useState("");
  const [selectedEvent, setSelectedEvent] = useState(null);
  const [loading, setLoading] = useState(true);
  const [view, setView] = useState(getDefaultView);
  const [currentDate, setCurrentDate] = useState(new Date());
  const [message, setMessage] = useState({ type: "", text: "" });

  useEffect(() => {
    loadTermini();
  }, []);

  useEffect(() => {
    function handleResize() {
      if (window.innerWidth < 900 && view === Views.MONTH) {
        setView(Views.AGENDA);
      }
    }

    window.addEventListener("resize", handleResize);
    return () => window.removeEventListener("resize", handleResize);
  }, [view]);

  async function loadTermini() {
    setLoading(true);
    setMessage({ type: "", text: "" });

    try {
      const response = await api.get("/Termin");
      setTermini(Array.isArray(response.data) ? response.data : []);
    } catch (error) {
      setMessage({
        type: "error",
        text: extractErrorMessage(
          error,
          "Neuspjesno ucitavanje kalendara termina.",
        ),
      });
    } finally {
      setLoading(false);
    }
  }

  const kabinetOptions = useMemo(() => {
    const uniqueKabineti = new Map();

    termini.forEach((termin) => {
      if (!termin.kabinetID) return;

      uniqueKabineti.set(termin.kabinetID, {
        id: termin.kabinetID,
        naziv: termin.kabinetNaziv || `Kabinet #${termin.kabinetID}`,
      });
    });

    return [...uniqueKabineti.values()].sort((left, right) =>
      left.naziv.localeCompare(right.naziv),
    );
  }, [termini]);

  const calendarEvents = useMemo(() => {
    return termini
      .map((termin) => {
        const start = toLocalDate(termin.datum, termin.vrijemePocetka);
        const end = toLocalDate(termin.datum, termin.vrijemeKraja);

        if (!start || !end) {
          return null;
        }

        const status = resolveTerminStatus(termin);

        return {
          id: termin.id,
          title: termin.kabinetNaziv || `Kabinet #${termin.kabinetID}`,
          start,
          end,
          status,
          statusLabel: STATUS_META[status].label,
          kabinetID: termin.kabinetID,
          kabinetNaziv: termin.kabinetNaziv || `Kabinet #${termin.kabinetID}`,
          kreatorIme: termin.kreatorIme || `Korisnik #${termin.kreatorID}`,
          rawTermin: termin,
        };
      })
      .filter(Boolean);
  }, [termini]);

  const filteredEvents = useMemo(() => {
    if (!selectedKabinetId) {
      return calendarEvents;
    }

    return calendarEvents.filter(
      (event) => String(event.kabinetID) === selectedKabinetId,
    );
  }, [calendarEvents, selectedKabinetId]);

  const eventCounts = useMemo(() => {
    return filteredEvents.reduce(
      (accumulator, event) => {
        accumulator[event.status] += 1;
        return accumulator;
      },
      { available: 0, occupied: 0, blocked: 0 },
    );
  }, [filteredEvents]);

  const activeKabinet = useMemo(() => {
    if (!selectedKabinetId) return null;
    return (
      kabinetOptions.find(
        (kabinet) => String(kabinet.id) === selectedKabinetId,
      ) || null
    );
  }, [kabinetOptions, selectedKabinetId]);

  function resetFilter() {
    setSelectedKabinetId("");
  }

  function eventPropGetter(event) {
    const meta = STATUS_META[event.status] || STATUS_META.available;

    return {
      className: `calendar-event calendar-event--${meta.className}`,
      style: {
        backgroundColor: meta.background,
        borderColor: meta.border,
        color: meta.color,
      },
    };
  }

  return (
    <Layout>
      <div className="page-header">
        <h1>Kalendar termina</h1>
      </div>

      <div className="calendar-page">
        <div className="card calendar-toolbar">
          <div className="calendar-filter-block">
            <label htmlFor="calendar-kabinet-filter">Kabinet</label>
            <select
              id="calendar-kabinet-filter"
              value={selectedKabinetId}
              onChange={(event) => {
                setSelectedKabinetId(event.target.value);
                setSelectedEvent(null);
              }}
            >
              <option value="">Svi kabineti</option>
              {kabinetOptions.map((kabinet) => (
                <option key={kabinet.id} value={kabinet.id}>
                  {kabinet.naziv}
                </option>
              ))}
            </select>
          </div>

          <div className="calendar-legend">
            {Object.entries(STATUS_META).map(([key, meta]) => (
              <div className="calendar-legend-item" key={key}>
                <span
                  className={`calendar-legend-dot calendar-legend-dot--${meta.className}`}
                />
                <span>{meta.label}</span>
                <strong>{eventCounts[key]}</strong>
              </div>
            ))}
          </div>

          {selectedKabinetId && (
            <button className="button sekundarno" onClick={resetFilter}>
              Resetuj filter
            </button>
          )}
        </div>

        {message.text && (
          <p className={message.type === "error" ? "form-error" : "form-success"}>
            {message.text}
          </p>
        )}

        <div className="calendar-board">
          <div className="card calendar-main-card">
            <div className="calendar-board-header">
              <div>
                <h2>{activeKabinet ? activeKabinet.naziv : "Svi termini"}</h2>
                <p>
                  {filteredEvents.length > 0
                    ? `Prikazano termina: ${filteredEvents.length}`
                    : "Nema termina za odabrani filter."}
                </p>
              </div>
            </div>

            <div className="calendar-wrapper">
              {loading ? (
                <div className="users-empty-state">Ucitavanje kalendara...</div>
              ) : filteredEvents.length > 0 ? (
                <Calendar
                  localizer={localizer}
                  events={filteredEvents}
                  startAccessor="start"
                  endAccessor="end"
                  titleAccessor="title"
                  style={{ height: "100%" }}
                  messages={CALENDAR_MESSAGES}
                  eventPropGetter={eventPropGetter}
                  view={view}
                  views={[Views.MONTH, Views.WEEK, Views.DAY, Views.AGENDA]}
                  date={currentDate}
                  onView={setView}
                  onNavigate={setCurrentDate}
                  onSelectEvent={setSelectedEvent}
                  popup
                />
              ) : (
                <div className="users-empty-state">
                  Nema termina za prikaz u kalendaru.
                </div>
              )}
            </div>
          </div>

          <div className="card calendar-sidebar-card">
            <h2>Detalji termina</h2>
            {selectedEvent ? (
              <div className="calendar-details">
                <p>
                  <strong>Kabinet:</strong> {selectedEvent.kabinetNaziv}
                </p>
                <p>
                  <strong>Datum:</strong> {formatDate(selectedEvent.start)}
                </p>
                <p>
                  <strong>Vrijeme:</strong> {formatTime(selectedEvent.rawTermin.vrijemePocetka)} -{" "}
                  {formatTime(selectedEvent.rawTermin.vrijemeKraja)}
                </p>
                <p>
                  <strong>Kreator:</strong> {selectedEvent.kreatorIme}
                </p>
                <p>
                  <strong>Status:</strong>{" "}
                  <span
                    className={`badge ${STATUS_META[selectedEvent.status].badgeClassName}`}
                  >
                    {selectedEvent.statusLabel}
                  </span>
                </p>
              </div>
            ) : (
              <div className="calendar-details-empty">
                Odaberi termin u kalendaru da vidis detalje.
              </div>
            )}

            <div className="calendar-help">
              <h3>Napomena</h3>
              <p>
                Boje termina prate status koji backend vrati za pojedini slot.
              </p>
            </div>
          </div>
        </div>
      </div>
    </Layout>
  );
}

export default Kalendar;
