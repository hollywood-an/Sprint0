"""Generates Content/adventurer.png, the original spritesheet for Sprint0.

The character art is authored below as ASCII pixel-art grids and written out
as a 64x128 RGBA PNG using only the Python standard library. Sheet layout is a
4-column x 8-row grid of 16x16 frames:

    row 0-3: walk Down, Up, Left, Right (4 frames each)
    row 4-7: idle Down, Up, Left, Right (2 frames each; last 2 columns empty)

Usage: python3 Tools/generate_spritesheet.py [--preview PATH SCALE]
"""

import struct
import sys
import zlib
from pathlib import Path

FRAME_SIZE = 16
SHEET_COLUMNS = 4
SHEET_ROWS = 8

PALETTE = {
    ".": (0, 0, 0, 0),
    "O": (34, 32, 52, 255),
    "H": (43, 111, 115, 255),
    "h": (30, 78, 82, 255),
    "S": (232, 193, 154, 255),
    "E": (24, 20, 37, 255),
    "T": (139, 94, 60, 255),
    "t": (105, 68, 42, 255),
    "B": (66, 44, 34, 255),
    "b": (212, 175, 84, 255),
    "L": (58, 52, 66, 255),
    "F": (110, 76, 48, 255),
    "G": (181, 137, 87, 255),
    "g": (146, 103, 60, 255),
}

BLANK_ROW = "................"

DOWN_BODY = [
    "................",
    "......OOOO......",
    ".....OHHHHO.....",
    "....OHHHHHHO....",
    "....OHHHHHHO....",
    "....OhSSSShO....",
    "....OhESSEhO....",
    "....OhSSSShO....",
    "...OtTTTTTTtO...",
    "...OtTTGTTTtO...",
    "...OBBbBGBBBO...",
    "...OBBBBBGGgO...",
]

UP_BODY = [
    "................",
    "......OOOO......",
    ".....OHHHHO.....",
    "....OHHHHHHO....",
    "....OHHHHHHO....",
    "....OHHHHHHO....",
    "....OhHHHHhO....",
    "....OhhhhhhO....",
    "...OtTTTTTTtO...",
    "...OtTTTGTTtO...",
    "...OBBGBBBBBO...",
    "...OGGgBBBBBO...",
]

LEFT_BODY = [
    "................",
    "......OOOO......",
    ".....OHHHHO.....",
    "....OHHHHHHO....",
    "....OHHHHHHO....",
    "....OSSHHHHO....",
    "....OSEHHHHO....",
    "....OShhhhhO....",
    "...OtTTTTTTtO...",
    "...OTGTTTTTtO...",
    "...OBbBBBBBBO...",
    "...OGGgBBBBBO...",
]

RIGHT_BODY = [
    "................",
    "......OOOO......",
    ".....OHHHHO.....",
    "....OHHHHHHO....",
    "....OHHHHHHO....",
    "....OHHHHSSO....",
    "....OHHHHESO....",
    "....OhhhhhSO....",
    "...OtTTTTTTtO...",
    "...OtTTTTTGTO...",
    "...OBBBBBBbBO...",
    "...OBBBBBBBBO...",
]

FRONT_LEGS = {
    "together": [
        ".....LL..LL.....",
        ".....LL..LL.....",
        ".....FF..FF.....",
        ".....FF..FF.....",
    ],
    "stepA": [
        ".....LL..LL.....",
        ".....FF..LL.....",
        ".....FF..FF.....",
        ".........FF.....",
    ],
    "stepB": [
        ".....LL..LL.....",
        ".....LL..FF.....",
        ".....FF..FF.....",
        ".....FF.........",
    ],
}

LEFT_LEGS = {
    "together": [
        "......LLL.......",
        "......LLL.......",
        ".....FFFF.......",
        ".....FFFF.......",
    ],
    "stepA": [
        "......LLL.......",
        ".....LL.LL......",
        "....FF...LL.....",
        "....FF....FF....",
    ],
    "stepB": [
        "......LLL.......",
        "......LL.L......",
        ".....FF..LL.....",
        ".....FF...FF....",
    ],
}

RIGHT_LEGS = {
    "together": [
        ".......LLL......",
        ".......LLL......",
        ".......FFFF.....",
        ".......FFFF.....",
    ],
    "stepA": [
        ".......LLL......",
        "......LL.LL.....",
        ".....LL...FF....",
        "....FF....FF....",
    ],
    "stepB": [
        ".......LLL......",
        "......L.LL......",
        ".....LL..FF.....",
        "....FF...FF.....",
    ],
}


def headBob(body):
    return [BLANK_ROW] + body[:7] + body[8:]


def buildFrame(body, legs):
    return body + legs


def walkFrames(body, legs):
    return [
        buildFrame(body, legs["stepA"]),
        buildFrame(headBob(body), legs["together"]),
        buildFrame(body, legs["stepB"]),
        buildFrame(headBob(body), legs["together"]),
    ]


def idleFrames(body, legs):
    return [
        buildFrame(body, legs["together"]),
        buildFrame(headBob(body), legs["together"]),
    ]


def validateFrame(frame, name):
    if len(frame) != FRAME_SIZE:
        raise ValueError(f"{name}: expected {FRAME_SIZE} rows, got {len(frame)}")
    for rowIndex, row in enumerate(frame):
        if len(row) != FRAME_SIZE:
            raise ValueError(f"{name} row {rowIndex}: expected {FRAME_SIZE} chars, got {len(row)}: {row!r}")
        for char in row:
            if char not in PALETTE:
                raise ValueError(f"{name} row {rowIndex}: unknown palette char {char!r}")


def buildSheetPixels():
    animationRows = [
        walkFrames(DOWN_BODY, FRONT_LEGS),
        walkFrames(UP_BODY, FRONT_LEGS),
        walkFrames(LEFT_BODY, LEFT_LEGS),
        walkFrames(RIGHT_BODY, RIGHT_LEGS),
        idleFrames(DOWN_BODY, FRONT_LEGS),
        idleFrames(UP_BODY, FRONT_LEGS),
        idleFrames(LEFT_BODY, LEFT_LEGS),
        idleFrames(RIGHT_BODY, RIGHT_LEGS),
    ]

    emptyFrame = [BLANK_ROW] * FRAME_SIZE
    pixels = []
    for sheetRow, frames in enumerate(animationRows):
        for frameIndex, frame in enumerate(frames):
            validateFrame(frame, f"sheet row {sheetRow} frame {frameIndex}")
        padded = frames + [emptyFrame] * (SHEET_COLUMNS - len(frames))
        for y in range(FRAME_SIZE):
            pixelRow = []
            for frame in padded:
                pixelRow.extend(PALETTE[char] for char in frame[y])
            pixels.append(pixelRow)
    return pixels


def writePng(path, pixels):
    height = len(pixels)
    width = len(pixels[0])
    raw = b"".join(
        b"\x00" + b"".join(bytes(pixel) for pixel in row) for row in pixels
    )

    def chunk(tag, data):
        payload = tag + data
        return struct.pack(">I", len(data)) + payload + struct.pack(">I", zlib.crc32(payload) & 0xFFFFFFFF)

    header = struct.pack(">IIBBBBB", width, height, 8, 6, 0, 0, 0)
    path.write_bytes(
        b"\x89PNG\r\n\x1a\n"
        + chunk(b"IHDR", header)
        + chunk(b"IDAT", zlib.compress(raw, 9))
        + chunk(b"IEND", b"")
    )


def scalePixels(pixels, factor):
    scaled = []
    for row in pixels:
        widened = [pixel for pixel in row for _ in range(factor)]
        scaled.extend([widened] * factor)
    return scaled


def main():
    repoRoot = Path(__file__).resolve().parent.parent
    outputPath = repoRoot / "Content" / "adventurer.png"
    pixels = buildSheetPixels()
    writePng(outputPath, pixels)
    print(f"wrote {outputPath}")

    if len(sys.argv) >= 3 and sys.argv[1] == "--preview":
        previewPath = Path(sys.argv[2])
        factor = int(sys.argv[3]) if len(sys.argv) > 3 else 8
        writePng(previewPath, scalePixels(pixels, factor))
        print(f"wrote {previewPath} at {factor}x")


if __name__ == "__main__":
    main()
