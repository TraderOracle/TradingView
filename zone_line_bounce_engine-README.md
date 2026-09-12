# TO Bounce Engine

A TradingView (Pine Script v6) indicator that draws a set of reference levels — EMA, session-anchored VWAP, opening range, Asia and London session extremes, plus any price zones you paste in — and fires **confirmed** bounce signals when price rejects off them.

The emphasis is on *confirmed*. Most level-touch indicators alert the moment a wick grazes a line. This one requires price to have been travelling toward the level beforehand, to close back out on the side it came from, and then to keep going in that direction for at least one more candle. A setup that fails any of those tests is discarded silently.

---

## Contents

- [Install](#install)
- [What appears on the chart](#what-appears-on-the-chart)
- [How a signal is produced](#how-a-signal-is-produced)
- [The four signal types](#the-four-signal-types)
- [Reading the markers](#reading-the-markers)
- [Settings reference](#settings-reference)
- [Pasting zones](#pasting-zones)
- [Alerts](#alerts)
- [How it works internally](#how-it-works-internally)
- [Limitations and gotchas](#limitations-and-gotchas)

---

## Install

1. Open TradingView, then **Pine Editor** at the bottom of the chart.
2. Paste the contents of `zone_line_bounce_engine_v7.pine`.
3. **Save**, then **Add to chart**.

Defaults are written for a **US Central (America/Chicago)** trader on an intraday timeframe. If you are in another timezone, see [Timezone & sessions](#timezone--sessions) — you must change the session strings, not just the dropdown.

Best on **1m to 15m** charts. The 1-hour opening range needs a timeframe that divides into an hour, and the bounce logic needs enough candles inside a session to be meaningful.

---

## What appears on the chart

| Element | Style | Where it comes from |
|---|---|---|
| **EMA 200** | Dotted, off-white, width 3, 20% transparent | `ta.ema(close, 200)` |
| **VWAP** | Solid, medium blue, width 3 | Anchored to the first bar of the NY session, reset daily |
| **1H opening range high / low** | Dotted white, width 1 | Highest high and lowest low of the first hour of the NY session |
| **Asia high / low** | Dashed light blue, width 1 | Extremes of the Asia session |
| **London high / low** | Dashed light pink, width 1 | Extremes of the London session |
| **Zones** | Filled teal boxes, full chart width | The `top,price,bottom,price` list you paste in |
| **Line titles** | Text to the right of each line, vertically centred on it | Named after the level, coloured to match |
| **Bounce markers** | Small triangles and diamonds | One confirmed bounce each |
| **Signal labels** | Small tags above/below the bar | Which levels fired; hover for full detail |

Session levels persist after their session closes and stay on the chart until that session next opens, so the Asia high remains a usable reference through London and New York.

Every line and zone can be switched off independently. Turning a line off also disables its signals.

---

## How a signal is produced

Four gates, in order. Failing any one of them ends the setup with no alert.

**1 · Ready.** Session-derived levels (opening range, Asia, London) stay inert until the session that produced them has closed. While a session is running its high is still being printed, so every new high "touches" it by construction — signalling there would be noise, not information. The EMA, VWAP and pasted zones are always ready.

**2 · Approach.** On the bar that first makes contact with the level, the preceding N candles (default 2) must have been travelling toward it. Measured on the extreme facing the level: lows for an approach from above, highs for an approach from below. Two conditions, both required — no candle in the run may back away from the level, and the contact bar must have made net progress versus the oldest candle in the run.

**3 · Bounce.** Price must close back out on the side it came from. Closing out the *other* side means the level broke; no signal, and the engine flips its idea of which side price is on.

**4 · Confirm.** The next N candles (default 1) must close further in the bounce direction than the bounce candle's own close, and must stay clear of the level. Fall back through it during the wait window and the pending signal is voided immediately.

Only after gate 4 does anything fire. There is no second chance on a failed setup — price has to leave the level and come back to arm a new one.

---

## The four signal types

Direction is set by **where price approached from**, not by candle colour. Arriving from above and bouncing back up is bullish (the level held as support). Arriving from below and being pushed back down is bearish (it held as resistance).

**1 · Wick rejection** — the wick pierced the level but no candle body ever entered it. The cleanest form of rejection.

**2 · Touch & bounce** — a body entered the level, but price closed back out the same side without reaching the far edge.

**3 · Back-of-zone bounce** — price traversed the zone, reached the far edge, rejected there, and exited back out the near side. **Zones only.** A single-price line has no far side, so lines can only produce types 1, 2 and 4.

**4 · Sweep** — price closed clean *through* the level, held on the far side for no more than X candles (default 2), then closed back through. A failed break: price made it past and gave it all back. This is the only type where price genuinely breached the level, which makes it structurally different from the three rejections above — worth treating separately in your own rules.

The breaking candle counts as the first one on the far side, so the default of 2 allows a reclaim on the next candle or the one after. Hold longer than that and the engine treats it as a genuine break and stops watching.

Types 1–3 are mutually exclusive and one interaction produces exactly one of them. A sweep is tracked by a separate watcher that runs alongside the interaction machine, because once price is on the far side any new contact reads as a fresh approach from that side and the reclaim would otherwise be invisible.

---

## Reading the markers

Colour identifies the level family, shape identifies the signal class, and placement above or below the bar carries direction.

| Marker | Meaning |
|---|---|
| ▲ below the bar | Bullish signal (level held, or a downside break was reclaimed) |
| ▼ above the bar | Bearish signal (level held, or an upside break was reclaimed) |
| Triangle | Rejection off a line — types 1 and 2 |
| ◆ Diamond | Rejection off a zone — types 1, 2 and 3 |
| ✕ Cross | Sweep — type 4, on any level |
| Off-white | EMA 200 |
| Medium blue | VWAP |
| White | Opening range |
| Light blue | Asia |
| Light pink | London |
| Teal | Pasted zone |

Markers sit on the **bounce candle**, not the confirmation candle, so the chart reads naturally. The alert still fires on the confirmation candle — nothing is signalled before it is known. Set *Draw marker on the bounce candle* to off if you would rather see the marker where the alert actually fired.

Labels carry short tags: `EMA`, `VW`, `ORH`, `ORL`, `ASH`, `ASL`, `LNH`, `LNL`, `Z`, each with ▲ or ▼. A leading `~` marks a sweep, so `~ASH▼` reads as "Asia high swept and reclaimed downward". Hover a label for the full description — level name, signal type, direction, and the zone price range where applicable.

---

## Settings reference

### Timezone & sessions

| Setting | Default | Notes |
|---|---|---|
| Timezone | `America/Chicago` | Every session string below is interpreted in this timezone |
| NY session | `0830-1500` | Anchors the VWAP at its first bar. CST default = 09:30–16:00 ET |
| 1H opening range | `0830-0930` | First hour of the NY session |
| Asia session | `1800-0300` | CST default = 19:00–04:00 ET |
| London session | `0200-1030` | CST default = 03:00–11:30 ET |

> **The dropdown and the session strings move together.** Switching the timezone does not translate the times — it changes how they are read. Pick `America/New_York` while leaving `0830-1500` in place and your NY session starts an hour early.

Session strings accept a day filter, e.g. `0830-1500:23456` for Monday–Friday.

### Lines

| Setting | Default | Notes |
|---|---|---|
| EMA 200 | on | Toggle, colour, and length on one row |
| Line transparency % | 20 | Applies to the EMA line only; its markers stay opaque |
| EMA dot spacing (bars) | 2 | 1 = a dot on every bar. Raise for a sparser line |
| VWAP (NY anchor) | on | Toggle and colour |
| 1H opening range | on | Toggle and colour, drives both the high and the low |
| Asia high / low | on | Toggle and colour |
| London high / low | on | Toggle and colour |
| Extend levels right (bars) | 3 | How far past the current bar the session lines run |

### Line titles

Each line gets its name printed to the right of it, vertically centred, in the line's own colour.

| Setting | Default | Notes |
|---|---|---|
| Show titles at right of each line | on | |
| Append the price | off | Adds the level's current value after the name |
| Text size | Small | Pine has no numeric font sizes. Small ≈ 9pt on a default chart |
| Font | Default | Only `Default` (the chart's sans-serif) and `Monospace` exist in Pine |
| Weight | Regular | Regular, Bold or Italic |
| Title offset right (bars) | 4 | Gap between the end of the line and its title |

> **Arial is not selectable.** Pine renders text with the system default font, which varies by operating system, and offers only a default and a monospace family. Sizes are enums, not point values.

### Zones

| Setting | Default | Notes |
|---|---|---|
| Enable zones | on | Off skips parsing entirely |
| Rectangles | sample list | See [Pasting zones](#pasting-zones) |
| Zone colour | teal | Fill is drawn at high transparency, border at low |

### Approach

| Setting | Default | Notes |
|---|---|---|
| Approach candles | 2 | Candles before first contact that must travel toward the level. **0 disables the filter** |
| Every approach candle must move closer | on | Off relaxes to net progress only, allowing a pullback mid-approach |

### Bounce rules

| Setting | Default | Notes |
|---|---|---|
| Session levels only signal once their session has closed | on | Leave on unless you deliberately want live-forming levels |
| Edge tolerance (price) | 0 | Widens every level. Also sets the effective thickness of the single-price lines |
| Far-side reach % (zones only) | 80 | How deep price must travel to qualify as a back-of-zone bounce. 100 = must tag the far edge exactly |
| Max bars inside band | 10 | Linger longer before resolving and it is consolidation, not a bounce |
| Cooldown bars per band | 0 | Suppress repeat signals from the same level for N bars |

> **Tolerance is worth tuning.** At `0` a bounce requires the wick to touch the exact price. That is fine for session highs and lows. For the EMA and VWAP you will probably want a tick or two, or near-misses get discarded.

### Sweep

| Setting | Default | Notes |
|---|---|---|
| Max candles beyond the band | 2 | How long price may hold past the level before a reclaim stops counting. The breaking candle is the first one |

### Confirmation

| Setting | Default | Notes |
|---|---|---|
| Wait candles | 1 | Candles to wait after the signal candle before it may fire |
| Confirm close must beat the signal candle's close | on | The core "is it actually moving away" test |
| Confirm candle must close in the signal direction | off | Stricter: also requires close > open (bullish) or close < open (bearish). Filters dojis |
| Draw marker on the signal candle | on | Off places markers on the confirmation candle instead |

### Signals

| Setting | Default | Notes |
|---|---|---|
| 1 · Wick rejection | on | |
| 2 · Touch & bounce | on | |
| 3 · Back-of-zone bounce | on | Zones only; has no effect on the lines |
| 4 · Sweep | on | Failed break that was reclaimed in time |
| Signal labels | on | Turn off for a cleaner chart; markers remain |

---

## Pasting zones

The **Rectangles** field takes a flat comma-separated list of keyword/price pairs:

```
top,7747.88,bottom,7746.17,top,7765.05,bottom,7763.34,top,7786.07,bottom,7781.99
```

- Spaces, tabs and line breaks are ignored, so a multi-line paste is fine.
- Order inside a pair does not matter — the higher number is always taken as the top.
- `bottom`, `bot` and `b` are all accepted. Any other non-numeric token is treated as `top`.
- A single price level can be entered as `top,7750,bottom,7750`.

Zones carry no timestamps, so each one spans the full chart width and is live across all history. Expect signals from well before the current session.

If nothing appears after pasting, a token has been mangled — check for a stray character or a missing comma between a keyword and its price.

---

## Alerts

There are **two separate mechanisms**, and they do not combine.

**Per-line conditions.** Eleven named entries appear in TradingView's alert dropdown:

`EMA 200 bounce` · `VWAP bounce` · `1H OR High bounce` · `1H OR Low bounce` · `Asia High bounce` · `Asia Low bounce` · `London High bounce` · `London Low bounce` · `Zone bounce` · `Any sweep` · `Any line or zone signal`

Each per-line entry fires on a confirmed signal off that level in either direction and of any type, sweeps included. `Any sweep` cuts the other way — every level, sweeps only. Their messages are **static** — Pine does not allow `alertcondition()` messages to be built at runtime.

**Dynamic detail.** Choosing **"Any alert() function call"** instead gives you a message naming the symbol, timeframe, which level was hit, the bounce type and the direction — but it triggers on every level, with no way to narrow it down.

So: target one level and accept a generic message, or get rich text for everything.

> **Set alerts to "Once Per Bar Close", not "Once Per Bar".** On the live forming candle the confirmation is evaluated against the running close, so it can flip on and off intrabar. Per-bar-close is the only setting that matches how the logic is designed.

---

## How it works internally

Everything price can bounce off is modelled as a **Band** — a top and a bottom price. A pasted rectangle is a genuine band. The EMA, VWAP and session levels are bands where top equals bottom, widened only by the tolerance setting. A single `step()` method drives all of them, so lines and zones share identical approach, bounce and confirmation logic.

Bands carry a `useFar` flag, off for lines, that disables back-of-zone detection. A zero-width band would otherwise register a far-side hit on every contact.

Passing `na` into `step()` parks a band: its state clears and nothing can fire. This is how the readiness gate suspends session levels while their session is still running, and how switching a line off disables it cleanly.

Pending signals live on the band itself, and are resolved at the top of each bar's processing — before any new interaction can open on that same band. Without that ordering, a level touched on consecutive bars would overwrite its own pending state.

The sweep watcher is a second piece of state that runs between the pending check and the interaction machine. A break starts its clock; a close back through the level stops it. It has to sit outside the interaction machine because once price is beyond the level, the machine correctly reads it as being on that side now — a later contact looks like a fresh approach from the far side, not a reclaim. A guard stops a reclaim that just armed a sweep from immediately arming the opposite one, which would otherwise ping-pong in choppy conditions.

Line titles are deleted and recreated on every last bar rather than created once and moved. Pine keeps only the newest `max_labels_count` labels and garbage-collects the rest, so titles created at bar 0 would be the first casualties once enough signal labels had accumulated.

---

## Limitations and gotchas

**Pine cannot read your hand-drawn rectangles.** Drawings made with the chart's drawing tools live in the chart layout, invisible to Pine — `box.all` and `line.all` contain only objects the script itself created. That is why zones are pasted as text rather than picked up from the chart. If you need true drawing access, either set alerts directly on a drawing (right-click → Add alert on drawing), or use TradingView's licensed Charting Library, where `getAllShapes()` exposes real coordinates.

**Daylight saving drift.** All four sessions are defined in one timezone. For two to three weeks each spring and autumn, US, European and Japanese DST transitions do not line up, and your Asia and London levels will be an hour off during those windows. Fixing it properly requires a separate timezone per session.

**VWAP needs volume.** On symbols without volume data the VWAP plot is blank and its signals never fire.

**Opening range needs a suitable timeframe.** On a 1H chart or higher the opening range collapses to a single candle or fails to resolve.

**Higher timeframes thin out signals.** The approach filter needs the preceding candles to be moving toward the level. On a 4H chart, two candles is most of a day of price action and the filter rarely passes.

**No repainting, with one caveat.** Signals are evaluated on closed bars and never move once printed. The live forming bar is the exception — see the alert frequency note above.

---

## License

MIT. Use it, modify it, ship it. No warranty; this is an analysis tool, not trading advice.
