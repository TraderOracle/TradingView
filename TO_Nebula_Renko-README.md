# Nebula (Renko Edition)

**Nebula** is a TradingView indicator that combines a trend cloud, candle coloring, and a set of reversal and continuation signals into one overlay. It was originally built by **TraderOracle** for the 1-minute chart.

This edition runs all of Nebula's math on a **standard 1-minute feed** (or any timeframe you choose). It then draws the results cleanly on a **Renko** chart, so you get Renko's noise-free bricks with the signals Nebula was designed around.

> ⚠️ **Disclaimer:** This is an educational tool, not financial advice. No indicator is right all the time. Practice on a simulator or paper account before risking real money, and always use a stop loss.

---

## Table of Contents

1. [Installation & Chart Setup](#installation--chart-setup)
2. [What's on the Screen (Symbol Guide)](#whats-on-the-screen-symbol-guide)
3. [Entering and Exiting Trades](#entering-and-exiting-trades)
4. [Trading the Nebula: Front and Back Bounces](#trading-the-nebula-front-and-back-bounces)
5. [Settings Reference](#settings-reference)
6. [Alerts](#alerts)
7. [How the Renko Mode Works](#how-the-renko-mode-works)
8. [FAQ / Troubleshooting](#faq--troubleshooting)
9. [Credits](#credits)

---

## Installation & Chart Setup

1. In TradingView, open the **Pine Editor** (bottom panel).
2. Click **Open → New indicator**, delete the template code, and paste in the contents of `Nebula_Renko.pine`.
3. Click **Save**, then **Add to chart**.
4. Switch the chart type to **Renko**:
   - Chart type dropdown → **Renko**
   - Chart settings → Symbol → **Box size assignment method: Traditional**, **Box size: 2** (or whatever suits your instrument)
5. Set the chart's **interval** (the timeframe dropdown) to **1m**. Renko is built from this interval, and 1m keeps brick timestamps lined up with the signal data.
6. **Important:** In chart settings → Symbol, **uncheck Body, Borders, and Wick**. Nebula paints its own colored candles, and the chart's default candles will cover them if left on.

That's it. You should now see the colored cloud and candles, with signal symbols appearing above and below the bricks.

---

## What's on the Screen (Symbol Guide)

### The Cloud (the "Nebula")

The shaded band that follows price is the Nebula. It is built from two moving averages:

| Edge | Line | Behavior |
|---|---|---|
| **Front** | Fantail VMA | Faster; hugs price more closely |
| **Back** | McGinley Dynamic | Slower; the "floor" (uptrend) or "ceiling" (downtrend) |

| Cloud color | Meaning |
|---|---|
| 🟩 **Green** | Uptrend: the front (fast) line is above the back (slow) line |
| 🟥 **Red** | Downtrend: the front line is below the back line |

- A **wide** cloud means a strong, established trend.
- A **thin or twisting** cloud means a weak trend, chop, or a possible reversal.
- With **Cloud Type** set to *Relative Strength*, *Money Flow*, or *Commodity Channel*, the cloud gets **brighter** as momentum strengthens.

### Candle Colors

Candles are colored by the mode chosen in **Candle Coloring** (default: **Waddah**).

| Mode | What bright colors mean |
|---|---|
| **Waddah** (default) | A bright, solid candle has explosive momentum in that direction. A faint or hollow candle has momentum present but below the "explosion" threshold, which is weak and not worth chasing. |
| **Vector** | Green/red candles have very high volume (climax or institutional activity). Blue/violet candles have above-average volume. Faint candles have normal volume. |
| **Squeeze** | Bright candles have growing squeeze momentum. Dim candles with no border have fading momentum. |
| **Volume Delta** | Brightness shows how strongly buyers (green) or sellers (red) dominate. |

### Signal Symbols

Green symbols appear **below** bricks (bullish). Red symbols appear **above** bricks (bearish).

| Symbol | Name | What it means |
|---|---|---|
| **`.`** green / **`•`** red | **Buy / Sell Signal** (basic) | The Tidal Wave has **flipped direction**: price has broken the previous wave's structure without overlapping it. This is a trend-change entry. |
| **➊** green / red | **Strong Buy / Strong Sell** | The same wave flip, **confirmed** by an *Ultimate Buy/Sell* signal within the last 4 bars. This is the highest-quality entry signal Nebula gives. |
| **`+`** | **Add Contract (Light)** | Inside an existing trend, a **volume imbalance** (a gap between candle bodies in the trend direction). The trend is continuing, so this is a reasonable place to add or enter late. |
| **✚** | **Add Contract (Strong)**, a.k.a. **Vodka Shot** | A **volume-backed continuation burst**: rising slow MA, strong directional energy, and a large volume surge. This is a stronger add signal than `+`. |
| **✓** purple | **Take Partial Profit** | At least 5 (default) reversal indicators are firing together against your trend. The move is getting stretched, so consider scaling out. |
| **✔** red | **Take ALL Profit** | At least 7 (default) reversal indicators are firing together. This is a strong exhaustion warning, so consider closing the position. |
| **▲ ▼** yellow | **9/21 EMA Cross** *(off by default)* | The fast EMA crossed the slow EMA (or the kernel regression line, if "quadratic" is on). This is a secondary trend confirmation. |
| **Dotted lines** *(off by default)* | **Volume Imbalances** | Price levels where an imbalance gap formed. Price often returns to "fill" these, and a line disappears once price trades through it. |
| **Fuchsia line** *(off by default)* | **HEMA** | A smooth trend line, useful as an extra trailing reference. |

> **Note on the take-profit names:** In the indicator's *Style* tab, the plot titles are swapped from the original script ("Take Full Profit" is the purple ✓). Go by the table above: **purple ✓ = partial, red ✔ = all**. This matches the thresholds in *Basic Settings*.

#### What counts toward the take-profit checkmarks?

Each of these reversal tools adds points when it fires on the current or previous bar:

| Tool | Points |
|---|---|
| Squeeze Relaxer (momentum fading after a squeeze run) | 4 |
| Trampoline (RSI + Bollinger Band snap-back) | 4 |
| LuxAlgo Reversal (9-count exhaustion) | 3 |
| Bollinger Band wick rejection | 2 |
| Dead Simple Reversal (engulfing at a 50-bar extreme) | 2 |
| Total Recall (vector candle at a structure break) | 2 |
| The Shark (RSI outside its own Bollinger Bands) | 2 |

The purple ✓ appears at **5 or more points** and the red ✔ at **7 or more** (both adjustable).

---

## Entering and Exiting Trades

This section describes one common way to trade Nebula. Adapt it to your instrument, risk tolerance, and experience.

### Before you trade: read the cloud

- **Trade with the cloud.** Look for longs when it's green and shorts when it's red.
- **Avoid thin, twisted, or rapidly flipping clouds.** That's chop, and signals there fail more often.
- **Check the candles.** Bright Waddah candles in your direction confirm momentum. Faint candles mean momentum is weak.

### Entries

**1. Trend-change entry (dot or ➊)**
- A **green ➊** is the best long entry: a wave flip plus an Ultimate Buy confirmation. A **green `.`** is the basic version.
- Enter on the brick where the signal appears, or on the next brick if it continues in your direction.
- Best when the signal agrees with the cloud color, or when the cloud is just turning.
- Treat signals **against** the cloud with caution. They're counter-trend trades.

**2. Continuation entry (`+` or ✚)**
- Missed the first entry? A **`+`** or **✚** in the trend direction is a chance to join or add.
- **✚ (Vodka Shot)** carries more weight because it's backed by a volume surge.

**3. Cloud bounce entry**
- See [Front and Back Bounces](#trading-the-nebula-front-and-back-bounces) below. These are often the cleanest entries of all.

### Stop loss placement

- **Longs:** below the **back** of the cloud, or below the recent swing low, whichever makes sense for your risk.
- **Shorts:** above the back of the cloud, or above the recent swing high.
- On Renko, a common approach is to allow **2–3 bricks** against you beyond your structure level.

### Exits

| Signal | Suggested action |
|---|---|
| **Purple ✓** (partial) | Take some profit off, and consider moving your stop to breakeven. |
| **Red ✔** (all) | Strong exhaustion. Close or aggressively tighten. |
| **Opposite dot or ➊** | The wave has flipped against you, so exit. |
| **Price closes through the back of the cloud** | Trend structure is broken, so exit. |
| **Cloud changes color** | The trend has reversed, so exit if you haven't already. |
| **Faint candles against a stretched move** | Momentum is fading, so tighten your stop. |

**Simple scaling plan example (2 contracts):**
1. Enter 1 contract on a **➊**.
2. Add 1 contract on a **✚** or a front-of-cloud bounce.
3. Close 1 contract on the **purple ✓**.
4. Close the rest on the **red ✔**, an opposite signal, or a close through the back of the cloud.

---

## Trading the Nebula: Front and Back Bounces

The cloud isn't only a trend filter. It also acts as **dynamic support in an uptrend** and **dynamic resistance in a downtrend**. Where price bounces tells you how strong the trend is.

```
UPTREND (green cloud)

  price ──╮      ╭──╮       ╭───   ← bounce off FRONT = strong trend
          ╰──╮ ╭─╯  ╰─╮   ╭─╯
 ═════════FRONT (Fantail VMA)═══════
 ░░░░░░░░░░░░ NEBULA ░░░░░░░░░░░░░
 ═════════BACK (McGinley)═══════════   ← bounce off BACK = weaker, last line
                                         close THROUGH back = trend likely over
```

### Front of the Nebula bounce (strongest)

- **What it is:** price is trending, pulls back, **touches or dips into the front edge** of the cloud, then turns and continues in the trend direction.
- **What it tells you:** buyers (in an uptrend) are stepping in early. The trend is **healthy and strong**.
- **How to trade it:**
  1. Cloud is green (for longs) and reasonably wide.
  2. Price pulls back to the front edge. Candles often turn faint here as selling momentum fades.
  3. **Enter when a brick turns back in the trend direction** off the front edge, ideally with a bright candle, a `+`, or a ✚.
  4. Put your stop just beyond the **back** of the cloud.
- The same applies in reverse for shorts in a red cloud: price rallies up to the front edge and rolls over.

### Back of the Nebula bounce (weaker)

- **What it is:** the pullback goes **all the way through the cloud** to the back edge, then bounces.
- **What it tells you:** the trend is **still intact but weakening**. This is often the last good bounce before the cloud flattens or flips.
- **How to trade it:**
  1. Wait for **confirmation**, such as a bright candle in the trend direction, a dot or ➊, or a clear reversal brick off the back edge.
  2. Consider **smaller size** or **quicker profit-taking** than on a front bounce.
  3. The stop goes just beyond the back edge. There's no support behind it.

### When the bounce fails

- **Price closes through the back of the cloud:** the trend is likely over. Don't keep buying dips.
- **The cloud narrows and changes color:** a new trend may be starting. Look for a dot or ➊ in the new direction, then trade **front bounces of the new cloud**.

### Quick reference

| Pullback reaches… | Trend strength | Aggressiveness |
|---|---|---|
| Doesn't reach the cloud | Very strong (a runaway move) | Use `+` / ✚ to join; don't chase extended moves |
| **Front edge** | Strong | Full-size entry |
| Middle of cloud | Moderate | Wait for confirmation |
| **Back edge** | Weakening | Reduced size, quicker targets |
| Closes through back | Broken | No trend trades; wait for a new signal |

> **Tip:** Increase **Cloud smoothing** if the edges look jagged on your bricks. A smoother cloud makes front and back touches much easier to judge.

---

## Settings Reference

### Renko Mode

| Setting | Default | Description |
|---|---|---|
| Signal timeframe | 1m | The timeframe all Nebula calculations run on. |
| Only use closed signal-TF bars | On | Prevents repainting. A signal may appear up to one signal-TF bar later. |
| Cloud smoothing (bricks) | 8 | EMA smoothing applied to the cloud across bricks. Set to 1 to turn it off. |

### Visible Settings

| Setting | Default | Description |
|---|---|---|
| Cloud Type | Simple | *None*, *Simple* (flat color), or momentum-shaded (*Relative Strength*, *Money Flow*, *Commodity Channel*). |
| Candle Coloring | Waddah | *None*, *Vector*, *Waddah*, *Squeeze*, *Volume Delta*. |
| Color Theme | Standard | *Standard*, *Pinky and the Brain*, *Color Blind*, *Mellow Yellow*. |
| Show HEMA line | Off | Fuchsia trend line. |
| Show plus sign to add | On | The `+` volume-imbalance signal. |
| Show bigger plus sign (Vodka Shot) | On | The ✚ signal. |
| Show take profit suggestions | On | The ✓ / ✔ marks. |
| Show 9/21 EMA cross | Off | Yellow triangles. |

### Basic Settings

| Setting | Default | Description |
|---|---|---|
| Ignore dojis | Off | Skip tiny-bodied candles in the wave logic (designed for NQ, 1m). |
| Minimum signals for take partial profit | 5 | Points needed for the purple ✓. |
| Minimum signals for take ALL profit | 7 | Points needed for the red ✔. |
| Body size to consider a doji | 1 | Used when *Ignore dojis* is on. |

### Tidal Wave Settings

| Setting | Default | Description |
|---|---|---|
| Show volume imbalances | Off | Draws dotted lines at imbalance levels. |
| Number of bars to extend line | 50 | How far each line extends to the right, in bricks. |
| Line Width / Style | 3 / Dotted | Appearance of the imbalance lines. |

The remaining groups (WAE, Trampoline, Squeeze, Shark, Ultimate Buy Sell, Fantail VMA, HEMA, and others) tune the underlying tools. **New users should leave them at their defaults.**

---

## Alerts

Right-click the chart → **Add alert** → Condition: **Nebula vS (Renko)** → choose one of:

| Alert | Fires on |
|---|---|
| Buy Signal Basic / Sell Signal Basic | `.` / `•` |
| Buy Signal Super / Sell Signal Super | ➊ |
| Small Plus Sign (Volume Imbalance) | `+` |
| Large Plus Sign (Vodka Shot) | ✚ |
| Take Partial Profit | ✔ red (7+ points) |
| Take FULL Profit | ✓ purple (5+ points) |
| Ultimate Buy/Sell Signal | The underlying Ultimate Buy/Sell confirmation |
| 9/21 EMA Cross | Yellow triangles |

> The two take-profit alert names follow the original script's labels. Check which checkmark you actually want before you set one.

---

## How the Renko Mode Works

Renko bricks aren't time-based: one brick can span ten minutes, or one fast minute can print five bricks. Indicators calculated directly on Renko data behave very differently from the 1m chart they were designed for.

This edition works around that in four ways:

1. **Every calculation runs on a standard 1m chart** of the same symbol, never on the Renko bricks.
2. **Each signal is tracked as a running count** on the 1m side. When the count increases between two bricks, the symbol is drawn on the brick where it arrived. Nothing is lost when many 1m bars fall inside one brick, and nothing is duplicated when one minute creates several bricks.
3. **Cloud and candle colors** show the most recent 1m value at each brick. The cloud is then smoothed across bricks.
4. **No repainting (default).** Signals only use closed 1m bars, so what you see historically is what you would have seen live.

---

## FAQ / Troubleshooting

**The colored candles don't show.**
Uncheck *Body*, *Borders*, and *Wick* in chart settings → Symbol.

**Signals look different from my 1m chart.**
A signal appears on the brick that formed at or after the 1m bar where it fired. With *Only use closed signal-TF bars* on, it can appear up to one minute later than on the 1m chart.

**The cloud is jagged.**
Raise *Cloud smoothing*, for example to 12–15.

**Too many signals in chop.**
Only trade signals that agree with the cloud color, prefer ➊ over basic dots, and skip thin or twisted clouds.

**Can I use a different signal timeframe?**
Yes. Set *Signal timeframe* to 2m, 5m, and so on. Keep the chart's own interval at or below it.

---

## Credits

- **Nebula** created by **TraderOracle** (DaveTrade55 on TradingView). YouTube: [@traderoracle](https://www.youtube.com/@traderoracle)
- Inspired by Daviddtech's ["best indicators" video](https://www.youtube.com/watch?v=zG7oRD_upt4)
- Tidal Wave concept: **Aaron D**. YouTube: [@aarond98](https://www.youtube.com/@aarond98)
- Rational Quadratic Kernel idea: **@davidclarke6612**
- Component indicators: LuxAlgo (Reversal Signals, Market Structure), LazyBear (Waddah Attar Explosion), Bixord (Fantail VMA), Ankit_1618 (Cumulative Volume Delta), DGT (Neglected Volume), RedK (VADER, Slow Smooth WMA), TradersReality (PVSRA vector candles), "Serious Backtester" (Trampoline)

Licensed under the [Mozilla Public License 2.0](https://mozilla.org/MPL/2.0/).
