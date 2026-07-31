from reportlab.pdfgen import canvas
from reportlab.lib.pagesizes import A4
from reportlab.lib.units import mm
from PIL import Image
import os
import tempfile

# =========================
# CONFIG
# =========================
INPUT_DIR = "./"
OUTPUT_FILE = "qr_sheet.pdf"

PAGE_W, PAGE_H = A4

MARGIN_X = 10 * mm
MARGIN_Y = 10 * mm

CELL_W = 50 * mm
CELL_H = 60 * mm

QR_SIZE = 40 * mm
LABEL_OFFSET = 4 * mm

COLS = int((PAGE_W - 2 * MARGIN_X) // CELL_W)
ROWS = int((PAGE_H - 2 * MARGIN_Y) // CELL_H)

# =========================
# PDF
# =========================
c = canvas.Canvas(OUTPUT_FILE, pagesize=A4)

files = sorted([f for f in os.listdir(INPUT_DIR) if f.endswith(".png")])

def draw_cell(c, x, y, img_path, label):
    qr_x = x + (CELL_W - QR_SIZE) / 2
    qr_y = y + (CELL_H - QR_SIZE) / 2 + 5 * mm

    c.drawImage(
        img_path,
        qr_x,
        qr_y,
        width=QR_SIZE,
        height=QR_SIZE,
        preserveAspectRatio=True,
        mask='auto'
    )

    c.setFont("Helvetica", 8)
    c.drawCentredString(
        x + CELL_W / 2,
        qr_y - LABEL_OFFSET,
        label
    )

# =========================
# TEMP FOLDER HANDLING
# =========================
with tempfile.TemporaryDirectory() as tmpdir:

    col = 0
    row = 0

    for f in files:
        path = os.path.join(INPUT_DIR, f)
        name = os.path.splitext(f)[0]

        # Convert PNG → temporary JPEG (safe for ReportLab)
        img = Image.open(path).convert("RGB")

        tmp_path = os.path.join(tmpdir, f"{name}.jpg")
        img.save(tmp_path, "JPEG", quality=95)

        x = MARGIN_X + col * CELL_W
        y = PAGE_H - MARGIN_Y - (row + 1) * CELL_H

        draw_cell(c, x, y, tmp_path, name)

        col += 1
        if col >= COLS:
            col = 0
            row += 1

        if row >= ROWS:
            c.showPage()
            col = 0
            row = 0

    c.save()

print(f"Created: {OUTPUT_FILE}")
print("Temporary images cleaned automatically.")