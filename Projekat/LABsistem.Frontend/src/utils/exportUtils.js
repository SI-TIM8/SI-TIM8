import jsPDF from "jspdf";
import autoTable from "jspdf-autotable";

function formatDate(value) {
  if (!value) return "N/A";
  return new Date(value).toLocaleDateString("de-DE");
}

function formatTime(value) {
  if (!value) return "";
  return value.slice(0, 5);
}

// ────────────────────────────────────────────
//  CSV
// ────────────────────────────────────────────

function buildCSV(headers, rows) {
  const escape = (val) => `"${String(val ?? "").replace(/"/g, '""')}"`;
  const lines = [headers.map(escape).join(","), ...rows.map((r) => r.map(escape).join(","))];
  return lines.join("\n");
}

function downloadCSV(filename, csv) {
  const blob = new Blob(["\uFEFF" + csv], { type: "text/csv;charset=utf-8;" });
  const url = URL.createObjectURL(blob);
  const a = document.createElement("a");
  a.href = url;
  a.download = filename;
  a.click();
  URL.revokeObjectURL(url);
}

// ────────────────────────────────────────────
//  PDF helper
// ────────────────────────────────────────────

function buildPDF(title, headers, rows) {
  const doc = new jsPDF({ orientation: "landscape" });
  doc.setFontSize(14);
  doc.text(title, 14, 16);
  doc.setFontSize(9);
  doc.setTextColor(120);
  doc.text(`Generirano: ${new Date().toLocaleDateString("de-DE")}`, 14, 22);
  doc.setTextColor(0);

  autoTable(doc, {
    startY: 27,
    head: [headers],
    body: rows,
    styles: { fontSize: 9, cellPadding: 3 },
    headStyles: { fillColor: [59, 130, 246] },
    alternateRowStyles: { fillColor: [245, 247, 250] },
  });

  return doc;
}

// ────────────────────────────────────────────
//  Rezervacije export
// ────────────────────────────────────────────

const REZERVACIJE_HEADERS = ["Datum", "Početak", "Kraj", "Kabinet", "Profesor", "Status"];

function rezervacijeToRows(rezervacije, uloga) {
  return rezervacije.map((t) => [
    formatDate(t.datum),
    formatTime(t.vrijemePocetka),
    formatTime(t.vrijemeKraja),
    t.kabinetNaziv ?? "",
    uloga === "student" ? (t.profesorIme ?? "") : "",
    t.statusTermina ?? "",
  ]);
}

export function exportRezervacijeCSV(rezervacije, uloga) {
  const rows = rezervacijeToRows(rezervacije, uloga);
  const csv = buildCSV(REZERVACIJE_HEADERS, rows);
  downloadCSV("rezervacije.csv", csv);
}

export function exportRezervacijePDF(rezervacije, uloga) {
  const rows = rezervacijeToRows(rezervacije, uloga);
  const doc = buildPDF("Lista rezervacija", REZERVACIJE_HEADERS, rows);
  doc.save("rezervacije.pdf");
}

// ────────────────────────────────────────────
//  Historija export
// ────────────────────────────────────────────

const HISTORIJA_HEADERS = ["Datum", "Početak", "Kraj", "Kabinet", "Profesor"];

function historijaToRows(termini) {
  return termini.map((t) => [
    formatDate(t.datum),
    formatTime(t.vrijemePocetka),
    formatTime(t.vrijemeKraja),
    t.kabinetNaziv ?? "",
    t.profesorIme ?? "",
  ]);
}

export function exportHistorijaCSV(termini) {
  const rows = historijaToRows(termini);
  const csv = buildCSV(HISTORIJA_HEADERS, rows);
  downloadCSV("historija-termina.csv", csv);
}

export function exportHistorijaPDF(termini) {
  const rows = historijaToRows(termini);
  const doc = buildPDF("Historija termina", HISTORIJA_HEADERS, rows);
  doc.save("historija-termina.pdf");
}
