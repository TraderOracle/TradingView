# NQ Chart Framework — README

A TradingView port of the NinjaTrader NQ day-trading template described in
*Charts – NQ* (update 8-28-26). Three Pine v6 scripts that together rebuild the
whole chart: the EMA stack, SuperTrend dots, High Volume Box, pivots and
Camarilla, Initial Balance extensions, the hundred-level zones, and the two
oscillator panels.

| File | Pane | What it draws |
|---|---|---|
| `1_NQ_Framework_Price_Pane.pine` | Price (overlay) | EMAs, SuperTrend, VWAP, HVB, pivots, Camarilla, IB, ORB, session levels, 100-zones, signals, status table |
| `2_anaTSI_Universal_clone.pine` | Sub-pane 2 | TSI (pink), signal (white), histogram, zero line, ±band |
| `3_RSI_9_3_Banded.pine` | Sub-pane 3 | RSI 9,3 with the 50/20 band and the 80/60/40 lines |

Three scripts rather than one because TradingView allows a script only one
sub-pane. `SETUP_NOTES.md` covers the porting decisions, the plot-budget
rework and the known differences from NinjaTrader — read it if something looks
different from your NinjaTrader chart.

These are charting tools, not trade signals, and nothing here is financial
advice. The source document is explicit that the judgement calls are not
mechanical; the section on discretion at the end of this README is the part
that matters most.

---

## 1. Installation

1. Open the Pine Editor, choose **Open → New indicator**, paste the contents of
   a file, **Save**, then **Add to chart**. Repeat for all three.
2. Drag the two oscillators into separate panes and order them **TSI above
   RSI**, matching the NinjaTrader layout.
3. Set the chart timezone to **New York** (chart settings → Symbol → Timezone).
   The scripts build every level in exchange time regardless of this, but it
   keeps the clock on your axis matching the references in this document.
4. Open a 1-minute and a 5-minute NQ chart side by side. The framework is built
   for those two timeframes.

Everything below assumes the defaults as shipped.

### Timeframe behaviour

The 5-minute is the structural chart and the 1-minute is the working chart. The
scripts adapt:

- The **9 EMA draws only on 1-minute and faster**, because that is the only
  place the source method uses it. Turn off *Only draw EMA 1 on 1-minute and
  faster* if you want it everywhere.
- The **HVB is always computed from 5-minute bars** and drawn on whatever
  timeframe you are viewing. Put the 1-minute up and you get the same box as the
  5-minute, which is the "transpose it to the one minute" step done for you.
- Everything else (pivots, IB, ORB, session levels) is timeframe-independent by
  construction.

---

## 2. The strategy in one paragraph

Price spends the day moving between **landmarks** — horizontal levels that were
set by something real: yesterday's range, the overnight range, the first hour,
the highest-volume bar of the session, the Camarilla bands, the round-number
zones. The EMAs, SuperTrend, TSI and RSI don't tell you where price is going;
they tell you whether the move *into* a landmark has strength behind it, and
whether the move *away* from it is holding. A trade is a landmark plus a
momentum read that agrees with it. When the landmark and the momentum disagree,
that is information too — usually to stand aside or tighten up.

The single most important habit the source document describes: **know which
landmark price is currently between**, at all times. The status table in the top
right is there for exactly that.

---

## 3. Reading the chart

The source document works through five areas in order. Here is each one, what
the script draws, and what you are looking for.

### 3.1 The EMA stack

Four EMAs: **9** (1-minute only), **21**, **120**, **200**. All are green when
up-sloped and red when down-sloped. The 21 and 120 are the two that matter; the
200 is tertiary.

The simplest read in the whole framework:

> Long when the 21 EMA is crossed from below and **held**. Short when it is
> broken and price **closes below**. The cross generally happens on the 1-minute
> first.

The word "held" is doing the work. A wick through the 21 is not a cross. The
5-minute 21 EMA in particular is described as hard to break from the downside
and hold — so a 1-minute cross that the 5-minute rejects is a lower-quality
signal than one both agree on.

On the 1-minute the 9 EMA is used differently: during down moves it acts as
resistance, and price bouncing off it on the way down is normal continuation
rather than a reversal.

**Line widths.** Pine cannot draw dashed plots, so the four EMAs are separated
by width instead of by NinjaTrader's dotted/dashed/solid/dash-dot: 1 for the 9,
3 for the 21, 2 for the 120, 1 for the 200. The 21 is the thickest line on the
chart on purpose.

### 3.2 SuperTrend dots

Red dots above price are **resistance**. Blue dots below price are **support**.
They flip more often on the 1-minute than the 5-minute.

Use them as entry levels — shorts off red dots, longs off blue dots — with
stops. Two specific observations from the source:

- Price **breaking under the red dots** on the 5-minute puts a short in play.
- When price hits the red dots on the **1-minute and those dots are near the
  high of day**, price generally goes down. That was the 8/28 example where it
  made a new low of day afterwards.

**This is the one component you must calibrate.** See §6.

### 3.3 The two oscillator panes

**anaTSI Universal (pane 2).** Pink is the TSI, white is the signal line, and
the blue histogram is pink minus white (confirmed against your 7/16 chart, where
the axis read pink 14.19, white 21.5, histogram −7.31).

- Pink crossing **over** white = strength coming into price.
- Pink crossing **under** white = weakness showing up.
- The **red dashed zero line** is the important one. Crossed upward = sign of
  strength. Crossed downward = sign of weakness.
- The grey dashed band sits at +25 and the green dashed at −20, measured off
  your charts.

**RSI 9,3 (pane 3).** Banded by **50 on top and 20 on the bottom** — that purple
fill is the band. Plus reference lines at 80 (red), 60 (yellow dashed) and 40
(peach dashed).

- **RSI crossing 50 while pink crosses white** is the tell that the 1-minute 21
  EMA is about to be crossed. That combination is the primary entry trigger.
- **RSI breaking back up through 20**, particularly on the 5-minute, means
  expect price to break up. But this can take time, and price can fall 50 to
  100+ points while you wait. The band break is a *heads-up*, not an entry.
- **RSI at 80+ on the 5-minute** means expect corrective action at some point.
  But it can run into the 90s and stay there on a trend day, so confirm weakness
  with something else before fading it.

### 3.4 Pivots and Camarilla

Standard floor pivots (PP, R1–R3, S1–S3) plus all eight Camarilla levels.
**CR3 (magenta) and CS3 (aqua) are the two that matter** and are drawn thick and
dashed so they stand out.

Think of CR3 and CS3 as the day's containing rails:

- Many days price simply oscillates between them. Longs and shorts inside the
  range are valid.
- **Break under CS3 and hold** = sign of weakness.
- **Break over CR3 and hold** = sign of strength.

The pivot point itself is watched for support and resistance; the other pivot
levels are used as targets from above or below.

Two worked examples from the source document, useful as calibration for what
"working" looks like:

- 8/17/26: price walked from CR4 down to CS3 between 9:10am and 1pm Pacific,
  roughly 30280 to 30071.
- 8/23/26 at the 3pm Pacific reopen: CS3 → CR3 → CS3, from 29300.25 up to
  29480.50 (5 points past CR3) and back to 29301.25 (1.25 points past CS3).

### 3.5 The High Volume Box

The **highest-volume RTH 5-minute bar's high-to-low range**, drawn as a
rectangle and extended right. The script finds it automatically and prints the
NinjaTrader-style readout: high, low, midpoint, then volume and the bar's time.

The basic read: **price below the box is weakness, price above it is strength.**

There can be more than one box, usually around news. The script draws the top
two by volume:

- Second box **lower** than the first = generally a sign of weakness.
- Second box **higher** than the first = generally a sign of strength.

The classic setup is in §4.3.

### 3.6 VWAP

The yellow line, used as a **target** from below or above. Anchored to the RTH
open by default.

### 3.7 The hundred-level zones

Specific to NQ. Around every 100 handle there is a zone running from **95 to
06.25**, with **01.25** as the important number inside it.

- Coming from **above**, price often struggles to break under 06.25. If it does,
  expect 01.25, then 95, and mostly lower.
- Coming from **below**, the reverse.

The 7/14/26 example: price reached 29895.75 at 8:31am Pacific, could not hold,
and fell about 190 points to the SuperTrend blue dots near 29709 by 9:08am.

This happens often enough to be tradeable for scalps and for longer holds. The
zones are drawn as faint shaded bands with a dotted line at the X01.25 level.

### 3.8 Initial Balance and Opening Range

**IB** is the first hour. Take the high-low difference, halve it for the
midpoint, then extend up and down in 50% increments: ±50%, ±100%, and on trend
days ±150%, ±200%, ±250%. The script computes all of these (the 150/200/250 set
is behind a toggle).

The IB extensions earn their keep in one specific situation: **when price moves
outside the IB and away from most of the other horizontal lines on the chart**,
the extension levels become the targets. If those don't get touched, fall back
to the EMAs, SuperTrend, RSI and TSI, and check the longer-term charts.

**ORB** is the first 15 minutes — high, low and mid, the "15 orb" annotation on
your NinjaTrader charts.

### 3.9 Session and prior-day levels

Overnight high/low (ONHI, ONLO), London high/low (LH, LL), after-hours high/low
(AH, AL, off by default), prior day open/high/low/close, midnight open (midn)
and the RTH open (reo). These are the levels that get referenced constantly in
the worked examples — the 8/28 open pushed below both the overnight low and the
London low before reversing, and that reversal back up through them was the
first long of the day.

---

## 4. The setups

Five named patterns, drawn from the document. Each one is a landmark plus a
momentum read.

### 4.1 The confluence entry (the primary one)

**What:** RSI crosses 50, pink crosses white on the TSI, and the 1-minute 21 EMA
gets crossed.

**How it sequences:** the oscillators move first, the EMA cross confirms. On
8/28 the RSI broke over 20 at 6:33, the TSI flipped at 6:35, and price didn't
cross the 21 EMA until 6:37 — then it was tested twice before moving up. You can
take the early trade or wait for the 21 cross. More aggressive entry, worse fill
on the cross; that's the trade-off, and the document leaves it explicitly to the
trader.

**Script support:** green triangle below the bar when all three align long, red
triangle above when all three align short. Alerts `Long confluence` and
`Short confluence`.

**Invalidation:** price closes back on the wrong side of the 21 EMA.

### 4.2 The 21 EMA touch from above

**What:** the 1-minute 21 EMA is up-sloped (green), pink is holding above white
on the TSI, and price comes down and touches the 21 from above.

The document calls this a high-probability long. It is a continuation entry, not
a reversal — you are buying a pullback in an intact trend, and the two
conditions (EMA slope, pink above white) are what establish that the trend is
intact.

**Script support:** lime circle below the bar. Alert `21 EMA touch long`.

**Invalidation:** price closes below the 21, or pink loses white.

### 4.3 The HVB return-and-fail

**What:** price leaves the High Volume Box, comes back into it, and **fails to
reach the other side**. Price then generally continues in the direction of the
original move.

This is the classic setup in the source document and it is worth understanding
well, because it is a *failure* pattern — you are watching for something not to
happen.

The 6/18/26 walk-through: box set at 6:35am Pacific, 30615.50–30482.50 on 18.71k
volume. Price broke under to 30391 at 6:45, came back into the box, fell below
to 30400.5 (inside the 95–06.25 zone, note the two landmarks stacking), then ran
to the box top by 7:25. It hit a swing high, came back to the box bottom, made a
new swing high, then came down into the box **but not to the bottom** — that
partial retracement is the sign of strength. Price broke out of the box top at
9:00am and made the high of day at 12:50pm.

The counter-example, 8/14/26: price crossed between the box high and low **four
times** before moving down 140 points. Repeated full traversals are the
opposite of the setup — they mean neither side is failing, and the box is
containing rather than launching.

**Script support:** alerts `HVB break up` and `HVB break down` on the close
through either edge. The return-and-fail itself is a visual read; the script
gives you the box and the status table tells you whether you're above, below or
inside.

### 4.4 Hanging curtains

**What:** price comes up to a landmark from below, hits it, falls, comes back,
falls again — three, four, five times or more — before eventually breaking
through. An EMA is the usual landmark.

The 7/16/26 example: between 12:07pm and 12:44pm Pacific, price hit the 21 EMA
from below **seven times** on the 1-minute. At 12:51 it broke above and ran 100
points into the close.

The practical use is patience. Each rejection is a short entry with a tight stop
and a small target. The break, when it comes, is the bigger trade. What you want
to avoid is taking the fourth rejection with a wide stop and a big target.

There is no script marker for this — count them yourself.

### 4.5 The hundred-level rejection

**What:** price approaches a X00 zone from above, cannot break under 06.25, or
approaches from below and cannot break over 95.

When it does break, the sequence to expect is 06.25 → 01.25 → 95, "mostly
lower." The shaded zones and the dotted 01.25 line are on the chart for this.

This setup stacks well with others. On 8/28, 29706 acted as a ceiling three
separate times, and price failing at 29701.75 and again at 29703.5 while the TSI
and RSI printed *lower* readings each time was the tell that longs were failing.

---

## 5. Settings reference

### 5.1 Price pane — `1_NQ_Framework_Price_Pane.pine`

**1 · Sessions**

| Setting | Default | What it does |
|---|---|---|
| Session timezone | America/New_York | All levels are built in this timezone. Leave it. |
| RTH session | 0930-1600 | Drives the HVB search window, IB, ORB, RTH open and the session reset for every level. |
| Overnight (Globex) | 1800-0930 | Source for ONHI / ONLO. |
| London session | 0300-0930 | Source for LH / LL. Widen to 0200-0930 if you want the full European session. |
| After-hours session | 1600-1800 | Source for AH / AL. Off by default. |

**2 · EMAs**

| Setting | Default | Notes |
|---|---|---|
| Show EMAs | on | |
| EMA 1 / 2 / 3 / 4 | 9 / 21 / 120 / 200 | Change only if you are deliberately departing from the method. |
| Only draw EMA 1 on 1-minute and faster | on | The 9 is a 1-minute tool. Turn off to show it everywhere. |
| Up slope / Down slope | green / red | Applies to all four EMAs. |

**3 · SuperTrend dots**

| Setting | Default | Notes |
|---|---|---|
| ATR multiplier | 3.0 | **Calibrate this.** See §6. |
| ATR length | 10 | Secondary — the multiplier moves the flips far more. |
| Support / Resistance dots | aqua / red | |

**4 · VWAP**

| Setting | Default | Notes |
|---|---|---|
| Anchor | RTH open | Set to **Globex open** if you want VWAP running through the overnight, which is what your NinjaTrader charts show. |

**5 · High Volume Box**

| Setting | Default | Notes |
|---|---|---|
| Source timeframe | 5 | The bar size the volume search runs on. Leave at 5. |
| Also show 2nd-highest volume box | on | The second box carries the SOS/SOW read. |
| Show box 50% line | on | Dotted midline. |
| Show box readout | on | The high/low/mid + volume/time text block. |
| Extend box (bars) | 30 | How far right the box projects past the current bar. |

**6 · Pivots & Camarilla**

| Setting | Default | Notes |
|---|---|---|
| Show floor pivots | on | |
| Show Camarilla R1-4/S1-4 | on | |
| Built from | Daily (chart session) | Switch to **Previous RTH only** to build pivots from the 09:30–16:00 range instead of the full 23-hour day. This changes every level noticeably — pick one and stay with it. |
| CR3 / CS3 / Other Cam / Pivots | magenta / aqua / violet / orange | CR3 and CS3 are drawn at width 3 and dashed. |

**7 · Initial Balance**

| Setting | Default | Notes |
|---|---|---|
| IB length (minutes) | 60 | The document uses 1 hour. |
| Also show 150 / 200 / 250% | off | Turn on when you have identified a trend day. |

**8 · Opening Range**

| Setting | Default | Notes |
|---|---|---|
| Opening range (minutes) | 15 | Matches the "15 orb" annotation. |

**9 · Session / prior-day levels** — six on/off switches plus a colour. ONHI,
ONLO, LH, LL are on; AH/AL are off; prior day OHLC, midnight open and RTH open
are on.

**10 · Hundred-level zones**

| Setting | Default | Notes |
|---|---|---|
| Zone bottom offset from X00 | −5.00 | The "95" edge. |
| Zone top offset from X00 | +6.25 | The "06.25" edge. |
| Key level offset from X00 | +1.25 | The "01.25" line inside the zone. |
| Zones above / below price | 2 | Two above and two below the current 100 handle. |
| Zone width (bars back) | 250 | How far left the shading runs. |

**11 · Signals** — turn the markers off with *Show confluence markers*. The TSI
and RSI parameters here are **internal copies** used only to compute the
markers. If you change the settings in scripts 2 or 3, change them here too or
the arrows will disagree with the panes.

**12 · Style**

| Setting | Default | Notes |
|---|---|---|
| Show level name labels | on | The right-edge tags: PP, CR3, orb hi, 50% ib, ONLO and so on. |
| Label offset (bars right) | 3 | |
| Level line transparency | 0 | Raise to fade every horizontal level at once when the chart gets busy. Capped at 50. |
| Level length (bars back, 0 = since session open) | 0 | Levels run from the RTH open to the right edge. Set a fixed number to draw them further back for chart review. |
| Show status table / Position | on / Top right | |

### 5.2 anaTSI Universal — `2_anaTSI_Universal_clone.pine`

| Setting | Default | Notes |
|---|---|---|
| Long smoothing (r) | 25 | Standard TSI first smoothing. |
| Short smoothing (s) | 13 | Standard TSI second smoothing. |
| Signal EMA | 7 | **Worth a look.** Controls how far white lags pink. On your charts white peaks 3–4 bars after pink, which fits 7. Raise toward 13 if your white line turns too early. |
| Signal type | EMA | SMA available. |
| Upper band / Lower band | +25 / −20 | Measured off your charts. |
| Colour histogram by sign | off | Your charts use one flat colour. |
| Shade between pink and white | on | Green fill when pink leads, red when white leads. Not on the NinjaTrader original — turn off for a closer match. |
| Mark pink/white crosses | on | |

### 5.3 RSI 9,3 — `3_RSI_9_3_Banded.pine`

| Setting | Default | Notes |
|---|---|---|
| RSI period | 9 | |
| Smooth | 3 | NinjaTrader's Smooth parameter. |
| Show the smoothed Avg line too | off | NinjaTrader's RSI carries two plots; only one green line is visible on your charts, so the Avg is hidden. |
| Overbought (red solid) | 80 | The "expect corrective action" line. |
| Upper mid (yellow dashed) | 60 | |
| Band top (blue solid) | 50 | The line the confluence entry keys off. |
| Lower mid (peach dashed) | 40 | |
| Band bottom | 20 | The band break level. |
| Shade the 50–20 band | on | |
| Mark 50 and 20 crosses | on | |

### 5.4 Alerts

Nine on the price pane: Long confluence, Short confluence, 21 EMA touch long,
SuperTrend flip up, SuperTrend flip down, HVB break up, HVB break down, CR3
break up, CS3 break down.

Four on the TSI: pink over white, pink under white, zero cross up, zero cross
down.

Five on the RSI: crossed 50 up, crossed 50 down, band break back up, lost the 20
band, reached 80.

Set them through the alert dialog with **Condition** set to the indicator name
and the alert chosen from the dropdown.

---

## 6. Calibrating the SuperTrend

This is the one component that is a genuine estimate rather than a
reconstruction. `anaSuperTrendU11`'s parameters are not public, and they cannot
be reverse-engineered from the dot positions on a chart — the gap between the
dots and price ranges from roughly 10 points to 85 depending on where in the
trend you sample, which is simply what a trailing stop does.

To calibrate:

1. Open a 5-minute NQ chart in TradingView and the same day in NinjaTrader.
2. Pick a day with two or three clean flips.
3. Compare the **bar on which the dots flip colour**, not the distance from
   price.
4. If TradingView flips **later** than NinjaTrader, lower the multiplier (try
   2.5, then 2.0). If it flips **earlier**, raise it (3.5, then 4.0).
5. Check a second day before settling.

Leave the ATR length at 10 until the multiplier is close; it has much less
effect on flip timing.

Two smaller items worth a glance while you are at it:

- **TSI signal EMA** (script 2). Compare the lag between pink and white against
  your NinjaTrader panel.
- **Volume.** The HVB depends on your data feed, and TradingView's continuous
  contract volume can differ from NinjaTrader's. Check a day you already know —
  6/18/26 should give 30615.50 / 30482.50 at 18.71k, 6:35am Pacific. If the box
  lands on a different bar, the levels are right but the source data differs,
  and you should trust your NinjaTrader chart.

---

## 7. A working routine

**Before the open**

1. Note where price sits relative to CR3 and CS3 — that frames the day.
2. Note the overnight high and low and the London high and low. The open often
   tests one of them.
3. Note the prior day high, low and close.
4. Identify the nearest 100-zones above and below.

**First hour**

5. The ORB (15 min) forms first, then the IB (60 min). Neither is tradeable
   until complete.
6. The HVB usually sets in the first few 5-minute bars. Once the box is drawn,
   the above/below read is live.

**During the session**

7. Check the status table. Know which landmarks price is between.
8. Wait for a landmark interaction — a test of a level, an EMA touch, a box
   edge, a zone boundary.
9. Read the TSI and RSI **at that moment**. Do they agree with the direction the
   landmark implies?
10. If yes, that is a trade. If no, that is information — usually to wait.
11. Targets are the next landmark in the direction of travel. If price is
    outside the IB and away from the other horizontal lines, the IB extensions
    become the targets.

---

## 8. Where discretion is required

The source document is direct about this and the scripts do not change it.

**The oscillators will fake you out on the 1-minute.** Sustained pink-above-white
is what an up-move needs, but a cross can trick you into exiting a good trade,
particularly after price has already moved a lot. On 8/21/26 the 1-minute TSI
crossed back and forth four times in about an hour while the 5-minute held pink
above white from 7:25am to 9:05am straight through. Same market, two different
answers. When it happens mid-trade you have to decide: exit, take a partial, or
hold. The scripts cannot make that call.

**The signal markers fire in exactly those situations.** They implement the
confluence conditions literally as written, which means the 8/21 sequence would
have painted several arrows. Treat a marker as a prompt to look at the chart,
not as an entry.

**Landmarks conflict.** The HVB, pivots, Camarilla and overnight levels will at
times point different directions. The document's answer is to take the trade
with tighter stops, or not take it at all — and, if it is too complex, to make
it simpler. Run replays with pieces switched off and see which combination
actually works for you. Every component in these scripts has an on/off toggle
for exactly that purpose.

**Nothing on the RSI or TSI tells you about a ceiling.** The 8/28 example is
worth re-reading: price could not break 29706 three times while the oscillators
still looked constructive. The only hint was that the TSI and RSI were printing
lower readings at each successive test. That divergence read is a human
judgement, not an indicator output.
