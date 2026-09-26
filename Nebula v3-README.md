# Nebula v3 — User Guide

Nebula is an all-in-one TradingView indicator, designed for the 1-minute NQ chart. It colors your candles, draws a trend cloud, marks entries, add-ons and take-profit points, flags RSI divergences, and sums everything up in a small dashboard.

This guide has three parts:

1. **What you see on the chart** — every marker, color and panel, and what it means.
2. **How the signals work** — the logic behind entries, add-ons and exits.
3. **Settings** — every option in the settings panel, group by group.

> **First-time setup:** Nebula paints its own candles. For them to show properly, open your chart's **Settings → Symbol** and untick the chart's own candle **Body**, **Borders** and **Wick**. (The "NOTE" checkbox at the top of Nebula's settings is just a reminder of this and does nothing when ticked.)

> **Reading a signal:** every signal marker is a label you can hover over. The tooltip explains exactly what triggered it and shows the cloud, wave, RSI, ADX and confluence on that bar.

---

## 1. What you see on the chart

### Candles

Nebula recolors every candle based on the **Candle Coloring** mode you choose. All modes use your **Color Theme**'s bullish and bearish colors. Brighter usually means stronger.

| Mode | What the color tells you |
|---|---|
| **Waddah** *(default)* | Momentum. Solid, bright candles mean momentum is breaking out; faint candles mean it's building but hasn't broken out. |
| **Vector** | Unusual volume. Full bull/bear color on climax volume (≥ 200% of the 10-bar average, or the largest spread × volume in 10 bars). Blue/violet on high volume (≥ 150%). Near-invisible otherwise. |
| **Squeeze** | Squeeze momentum. Bright with a solid border while momentum grows; faded while it fades. |
| **Volume Delta** | Estimated buying vs. selling pressure. Brighter the more one side dominates. |
| **Trend Agreement** | Bright when the candle moves with the cloud's trend; faded when it moves against it. Makes pullbacks easy to spot. |
| **Tidal Wave** | Colored by the current wave (see below). Bright only on bars that print a signal. |
| **Signal Confluence** | Glows brighter as reversal signals stack up toward a take-profit mark. |
| **RSI Heat** | Red when RSI is low, clear around 50, green when RSI is high. |
| **Kernel Slope** | Direction and steepness of a smooth kernel regression line. Very smooth; good for trend-following. |
| **Relative Volume** | Quiet bars fade, heavy-volume bars pop. |
| **ADX Strength** | Gray when the market is choppy, filling in with color as a trend strengthens. |
| **Heikin Ashi Trend** | Colored by Heikin Ashi direction, but keeps normal candle shapes. Few color flips. |
| **Stretch from Mean** | Brighter the further price is from its 20-bar average. Highlights overextended moves. |
| **Supertrend** | Two colors that only flip on a real trend break. Candles against the trend are dimmed. |
| **None** | Nebula doesn't draw candles. |

### The cloud

The shaded band on the price chart is the **trend cloud**. It's filled between two adaptive moving averages (a Fantail VMA and a McGinley Dynamic).

- **Bullish color** = trend up. **Bearish color** = trend down.
- With the **Simple** cloud the shade is constant. With the **Relative Strength**, **Money Flow** or **Commodity Channel** cloud, the shade gets stronger as that oscillator confirms the trend.

### Signal markers

| Marker | Where | Meaning |
|---|---|---|
| **•** | Below (buy) / above (sell) | **Buy / Sell.** The Tidal Wave has flipped direction. |
| **Solid badge** | Below (buy) / above (sell) | **Strong Buy / Sell.** A wave flip backed by a recent Ultimate Buy/Sell signal. Buys are a badge in your bullish color pointing up at the bar; sells are a darker red badge pointing down. Hover over it to see the bar's confluence score. |
| **+** | Below / above | **Add (light)** *(off by default).* A volume imbalance in the direction of the current wave; a place to add to a position. |
| **✚** | Below / above | **Add (strong) — "Vodka Shot".** Trend, momentum and a volume spike all agree with the current wave. |
| **✔** (red) | Above in an up-wave, below in a down-wave | **Take all profit.** Many reversal signals are firing at once. |
| **✓** (purple) | Same | **Take partial profit.** Several reversal signals are firing. |
| **Div** | Below (bullish) / above (bearish) | **RSI divergence** — price and RSI disagree. Placed on the swing bar. |
| **▲ / ▼** (yellow, small) | Below / above | **9/21 EMA cross** (off by default). |

Buy-side markers use your theme's bullish color; sell-side markers use its bearish color.

### Other drawings

- **Squeeze zones** — a soft, rounded shape that hugs price while the market is coiling (a volatility squeeze). It follows the Bollinger Bands, so you can watch the range pinch tighter. Optionally, when the squeeze ends, a small **⇡** or **⇣** arrow at its edge shows which way momentum broke out; hover over it to see how long the squeeze lasted.
- **Volume imbalance lines** *(off by default)* — dotted lines at the open of a candle that gaps past the previous same-color candle's close. Each line stays until price fills the gap (trades back to that previous close), then disappears.
- **Divergence lines** *(off by default)* — thin lines joining the two swing points of a divergence.
- **HEMA line** *(off by default)* — a smooth, trend-following line.

### Session levels

Thin horizontal lines at the day's key prices, with a small name tag at the right edge of the chart:

| Tag | Level | Line |
|---|---|---|
| **ORH / ORL** | High and low of the opening range (first hour of the regular session, 9:30–10:30 ET) | solid |
| **LonH / LonL** | High and low of the London session (3:00–8:00 ET) | dashed |
| **AsiaH / AsiaL** | High and low of the Asia session (6:00 PM–3:00 AM ET) | dotted |
| **Close** | Yesterday's regular-session close (4:00 PM ET) | dotted, neutral |
| **OR 1.5× / OR 2×** | Opening range extensions: 1.5× and 2× the range, projected in the breakout direction | short dotted ticks |

- **Colors follow your theme.** Highs use the bullish color and lows use the bearish color, drawn faint while untouched. Once price closes through a level it turns full strength, so you can see at a glance which levels have been taken.
- **While a session is forming**, a faint dotted bracket shows its range as it builds. When the session ends, the bracket collapses into the two lines.
- **Tags** show only the level's name. Hover over a tag to see its exact price. Levels within a few ticks of each other share one tag, for example `LonH · ORH`.
- **Opening range breakout:** a small ▲ or ▼ marks the first close outside the opening range. The 1.5× extension appears after the breakout, and 2× appears once price reaches 1.5×.
- **Today only:** Asia, London and the opening range reset at the 6:00 PM ET futures open, so old levels don't pile up.

### The dashboard

A compact panel (top right by default) that summarizes the indicator:

| Row | What it shows |
|---|---|
| **NEBULA** | Symbol and chart timeframe. |
| **Trend (cloud)** | ▲ Up / ▼ Down, from the cloud. |
| **Wave** | ▲ Up / ▼ Down, from the Tidal Wave. |
| **ADX** | Trend strength bar (full at ADX 50). Gray when choppy; leans green when buyers lead and red when sellers lead. |
| **RSI** | RSI(14) bar. Gray at 50, redder toward 0, greener toward 100. |
| **Confluence** | Progress toward the take-profit threshold (e.g. 3/7). Fills toward red in an up-wave (top risk) and green in a down-wave (bottom risk). |
| **SESSION LEVELS** | Opening range size compared with its 20-day average, the breakout direction and time, and the nearest level above and below price with the distance to each. |

Hover over any bar to see its exact value and what it means.

---

## 2. How the signals work

### The Tidal Wave (the core buy/sell engine)

Nebula tracks a **wave state** that is either *up* or *down*. It only changes on confirmed (closed) candles.

- **Wave flips up (•  Buy):** during a down-wave, a green candle opens at or above the close of an earlier green candle, without a strong red wave candle in between. Buyers have reclaimed ground, so the wave flips up.
- **Wave flips down (•  Sell):** the mirror image: during an up-wave, a red candle opens at or below the close of an earlier red candle.
- **Gap continuation:** a green candle that opens at or above the previous green candle's close (no overlap — a volume imbalance) also sets or keeps the wave up, and vice versa for red.

### Strong signals (solid badge)

A **Strong Buy** is a Tidal Wave buy that happens on the same bar as, or within 3 bars after, an **Ultimate Buy** signal. Strong Sell is the mirror image.

**Ultimate Buy / Sell** is a two-step engine. It isn't drawn on its own, but it feeds Strong signals and alerts:

1. **Watch.** Price or RSI stretches to an extreme and snaps back. Any of these arms a *buy watch*:
   - price crosses back above the lower Bollinger Band (20, 2 SD);
   - RSI(32) crosses above its own lower band (2 SD around a 32-bar WMA) or crosses above 25;
   - the candle high drops below the lower ATR band (10-bar WMA − 1.5 × ATR(30)).
2. **Trigger.** Within the next 35 bars, RSI(32) crosses above its basis, above 25, or above its 24-bar WMA. The trigger bar can't itself be a watch bar.

After a signal fires, all pending watches are cleared. Sells are the mirror image (upper bands, 75).

Hover over a Strong marker to see the bar's confluence score (see *Take-profit signals* below). On a Strong Buy after a down-wave, a high score means many reversal detectors agree that the down-move is exhausting. The score is also included in the alert message, for example `Strong Buy (confluence 6)`.

### Add-on signals

- **Add (+):** a gap candle (opens beyond the previous same-color candle's close) while the wave is already moving that way. A sign that buyers (or sellers) are pressing.
- **Add (✚, "Vodka Shot"):** all of the following on the same bar, while the wave is already moving that way:
  - a triple-smoothed 7-bar WMA is rising (falling for shorts);
  - the Directional Energy Ratio shows buyers in control and gaining (sellers for shorts);
  - volume spikes well above its 70-bar average;
  - the candle closes in the trade direction;
  - no other Vodka Shot in the previous 4 bars.

### Take-profit signals (✔ / ✓)

Nebula scores **reversal evidence**. Each of these seven detectors adds points when it fires on the current or previous bar:

| Detector | Points | What it looks for |
|---|---|---|
| **Squeeze momentum turn** | 4 | Squeeze momentum reverses after a run of at least 3 bars in one direction (outside a squeeze). |
| **Trampoline** | 4 | Within the last 5 bars, a candle closed outside the Bollinger Band with RSI(14) ≤ 25 (or ≥ 72), and now price reverses through the prior bar's high (or low). |
| **9-count exhaustion** | 3 | Nine bars in a row closing below (or above) the close four bars earlier (TD Sequential–style). |
| **Engulfing reversal at an extreme** | 2 | An engulfing candle right after price set a new 50-bar low (or high). |
| **Early reversal** | 2 | A high-volume vector candle breaking the latest fractal high (or low). |
| **Shark** | 2 | RSI(14) pushes outside its own 30-bar Bollinger Bands. Optionally only below 26 / above 74. |
| **Bollinger wick rejection** | 2 | A wick pierces the 2.5 SD band but the candle closes back inside. |

- **✓ Take partial profit** when the total reaches *Minimum signals for take partial profit* (default 5).
- **✔ Take all profit** when it reaches *Minimum signals for take ALL profit* (default 7).

The mark is placed on the side where the move may be ending: above the bar in an up-wave, below it in a down-wave. The dashboard's **Confluence** bar shows this running total live.

### RSI divergence

Nebula finds swing highs and lows in RSI(14) using a 5-bar lookback on each side, and compares consecutive swings 5–60 bars apart:

| Type | Price | RSI | Meaning |
|---|---|---|---|
| Bullish | Lower low | Higher low | Selling pressure fading — possible bottom |
| Bearish | Higher high | Lower high | Buying pressure fading — possible top |

A swing can only be confirmed once the *Pivot Lookback Right* bars have closed after it. So divergences (and their alerts) arrive **5 bars after** the swing, even though the label is drawn on the swing bar itself.

### 9/21 EMA cross

Marks where the 9 EMA crosses the 21 EMA. With *Use quadratic equation for 9/21 cross* on, the 9 EMA is compared with a smooth kernel regression line instead of the 21 EMA.

### Alerts

- **Any Nebula signal** *(recommended, always available)*: create an alert on Nebula and choose **"Any alert() function call"**. You get one message per bar listing every signal that fired, for example `Nebula: NQ1! 1 | Strong Buy, Add Long @ 21345.25`. Alerts fire once the bar closes.
- **Individual alerts** are also available: Ultimate Buy/Sell, Buy/Sell Basic, Buy/Sell Super, Small Plus Sign, Large Plus Sign, the two take-profit alerts, 9/21 EMA Cross, and Bullish and Bearish RSI Divergence.

> **Naming note:** In the Style tab and the individual alert list, the two take-profit entries are labeled the other way round from this guide. "Take Partial Profit" is the red ✔ (the higher threshold) and "Take Full Profit" is the purple ✓ (the lower threshold). The chart markers themselves behave as described above.

---

## 3. Settings

Settings are listed in the order they appear in the settings panel. Every on/off switch for what's drawn on the chart is at the top, in **Visible Settings**.

### (Top)

| Setting | Default | What it does |
|---|---|---|
| NOTE: Designed for 1m NQ. Uncheck body/wick/border… | off | Reminder only; has no effect. See *First-time setup* at the top of this guide. |

### Visible Settings

| Setting | Default | What it does |
|---|---|---|
| Candle Coloring | Waddah | Which candle mode to use (see *Candles* above). |
| Color Theme | Standard | Bull/bear color pair used everywhere: Standard, Pinky and the Brain, Color Blind, Mellow Yellow, Neon, Cyberpunk, Aurora, Ocean, Sunset, Nord, Dracula, Tokyo Night, Royal, Ice and Fire, Green Lantern, Superman, Wonder Woman, Hulk, Aquaman. |
| Cloud Type | Simple | None, Simple, Relative Strength, Money Flow or Commodity Channel. The last three shade the cloud by that oscillator. |
| Cloud Colors | Theme | Theme (matches your Color Theme) or one of 12 schemes: Aqua / Coral, Emerald / Crimson, Sky / Orchid, Lime / Tangerine, Gold / Violet, Teal / Magenta, Mint / Rose, Cobalt / Amber, Jade / Ruby, Turquoise / Tomato, Periwinkle / Peach, Ice / Lava. |
| Show dashboard | on | Shows the dashboard panel. |
| Show squeeze zones | on | Shades periods when a volatility squeeze is on. |
| Show RSI divergences | on | Shows bullish and bearish RSI divergence labels. |
| Show session levels | on | Shows the opening range, London, Asia and Close levels. |
| Show HEMA line | off | Draws the smooth HEMA trend line. |
| Show plus sign to add | off | Shows the light **+** add-on markers. |
| Show bigger plus sign (Vodka Shot) | on | Shows the strong **✚** add-on markers. |
| Show 9/21 EMA cross | off | Shows the small yellow cross arrows. |
| Show volume imbalances | off | Draws a line at the open of each gap candle, kept until the gap is filled. |

### Dashboard

| Setting | Default | What it does |
|---|---|---|
| Position | Top Right | Any of 8 positions around the chart. |
| Text Size | Small | Tiny, Small, Normal or Large. The bars scale with it. |
| Show session levels in dashboard | on | Shows the SESSION LEVELS section. |
| Background Transparency | 20 | 0 = solid, 100 = invisible. The panel follows your chart's background and text colors. |
| Bar length (blocks) | 10 | Number of blocks in every dashboard bar (10 = each block is 10%). |

### Squeeze Zones

| Setting | Default | What it does |
|---|---|---|
| Squeeze zone color | Blue, 85% transparent | Fill color and transparency of the squeeze zones. The outline is drawn in a stronger version of the same color. |
| Minimum squeeze length (bars) | 3 | Squeezes shorter than this aren't drawn. |
| Show squeeze release arrows | off | Shows the ⇡ / ⇣ breakout arrow at the end of each squeeze zone. |

### RSI Divergence

| Setting | Default | What it does |
|---|---|---|
| Draw divergence lines on price | off | Joins the two swing points with a line. |
| Divergence Transparency | 50 | Transparency of divergence labels and lines. |
| RSI Length | 14 | RSI used for divergences. |
| Pivot Lookback Left | 5 | Bars to the left that a swing must beat. |
| Pivot Lookback Right | 5 | Bars to the right that a swing must beat. This is also the confirmation delay. |
| Min Bars Between Pivots | 5 | Ignore swings closer together than this. |
| Max Bars Between Pivots | 60 | Ignore swings further apart than this. |

### Session Levels

| Setting | Default | What it does |
|---|---|---|
| Opening range | on, 09:30–10:30 | Shows ORH / ORL. The time window is New York time. |
| London | on, 03:00–08:00 | Shows LonH / LonL. |
| Asia | on, 18:00–03:00 | Shows AsiaH / AsiaL. |
| Yesterday's close | on, 09:30–16:00 | Shows Close: the last close of this regular session. |
| Opening range extensions (1.5× / 2×) | on | Shows the extension ticks after an opening range breakout. |
| Shade ranges while they form | on | Shows the faint bracket while a session is building. |
| Mark opening range breakout | on | Shows the ▲ / ▼ on the breakout bar. |
| Merge tags within (ticks) | 8 | Levels closer than this share one tag. |
| Tag distance from last bar | 4 | How far to the right of the last candle the tags sit. |
| Level transparency | 45 | How faint untouched levels are (0 = solid, 100 = invisible). Broken levels are always full strength. |

### Tidal Wave Settings

| Setting | Default | What it does |
|---|---|---|
| Number of bars to extend line | 50 | How far each volume imbalance line reaches to the right. |
| Line Width | 3 | Volume imbalance line thickness. |
| Line Style | Dotted | Solid, Dotted or Dashed. |

### Advanced

| Setting | Default | What it does |
|---|---|---|
| Minimum signals for take partial profit | 5 | Confluence points needed for **✓**. |
| Minimum signals for take ALL profit | 7 | Confluence points needed for **✔**. Also the "full" mark on the dashboard's Confluence bar. |
| Use quadratic equation for 9/21 cross | off | Compares the 9 EMA with a kernel regression line instead of the 21 EMA. |
| Simple Cloud Transparency | 80 | 0 = solid, 100 = invisible. Simple cloud only. |
| Weak Trend Transparency | 80 | Transparency when the oscillator shows a weak trend (oscillator clouds only). |
| Strong Trend Transparency | 50 | Transparency when the oscillator shows a strong trend (oscillator clouds only). |

### WAE (Waddah Attar Explosion, used by Waddah candles)

| Setting | Default | What it does |
|---|---|---|
| Sensitivity | 150 | Multiplier on the MACD change. Higher = candles light up more easily. |
| FastEMA Length | 20 | Fast EMA of the MACD. |
| SlowEMA Length | 40 | Slow EMA of the MACD. |
| BB Channel Length | 20 | Length of the Bollinger Bands used for the "explosion line". |
| BB Stdev Multiplier | 2.0 | Width of those Bollinger Bands. |

### Relaxing Settings

| Setting | Default | What it does |
|---|---|---|
| Squeeze Tolerance | 2 | How many momentum bars must run in one direction before a turn counts as a Squeeze reversal. Lower = more sensitive. |
| ADX Threshold for TTM Squeeze | 21 | Currently has no effect. |

### Shark Settings

| Setting | Default | What it does |
|---|---|---|
| Apply 25/75 RSI rule | off | Shark only fires when RSI is also below 26 or above 74. |

### Fixed values

These used to be settings and are now built in:

| Value | Fixed at |
|---|---|
| Take-profit marks (✔ / ✓) | Always shown |
| Candle color smoothing | 3 bars |
| Supertrend candles | Factor 3, ATR period 10 |
| Hidden RSI divergences | Off |
| ADX threshold for squeeze reversals | 0 (squeeze turns always count) |
| Volume Delta candle brightness top end | 300 |
| Signal details on hover | Always on |
| "Any Nebula signal" alert | Always available |

---

### Tips for new users

- Start with the defaults on a chart you know well. Watch how the **wave** (• markers) and the **cloud** agree or disagree.
- Trades taken **with** the cloud and away from nearby session levels tend to be the cleaner ones. The dashboard's *Nearest above / below* rows show how much room price has.
- If the chart feels busy, turn off the markers you don't use (plus signs, take-profit marks, divergences) and try a calmer candle mode such as **Heikin Ashi Trend** or **Supertrend**.
