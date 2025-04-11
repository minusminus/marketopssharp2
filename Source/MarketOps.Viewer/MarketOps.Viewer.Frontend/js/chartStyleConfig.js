// js/chartStyleConfig.js

// --- Konfiguracja Kolorów i Stylów ---
const styleConfig = {
    // Kolory świec
    candleIncreasingColor: 'white',
    candleDecreasingColor: 'black',
    candleBorderColor: 'black',
    candleBorderWidth: 1,

    // Kolor wykresu liniowego
    lineChartColor: 'blue',

    // Kolory wolumenu
    volumeColor: 'rgba(100, 100, 255, 0.7)',

    // Konfiguracja osi
    axisTitleFontSize: 12,
    axisTickFontSize: 10,
    // ... inne globalne ustawienia stylu, jeśli potrzebne

    // Domyślny layout osi
    defaultAxisLayout: {
        showgrid: true, // Pokaż siatkę
        gridcolor: '#e1e1e1',
        // zeroline: false, // Czy pokazywać linię zero
        titlefont: { size: 12 },
        tickfont: { size: 10 }
    }
};

// --- Funkcje generujące definicje dla Plotly ---

/**
 * Generuje konfigurację layoutu dla głównego wykresu OHLCV + Wolumen.
 * @param {object | null} stockInfo Informacje o spółce (dla tytułu).
 * @returns {object} Obiekt layoutu Plotly.
 */
function getOhlcvLayout(stockInfo) {
    return {
        title: `Wykres dla ${stockInfo?.displayText || 'Wybrany instrument'}`,
        xaxis: {
            ...styleConfig.defaultAxisLayout, // Rozszerz o domyślne ustawienia osi
            rangeslider: { visible: false },
            type: 'category',
			rangebreaks: [ { bounds: ["sat", "mon"] } ],
            //tickangle: -45 // Można dodać, jeśli etykiety się nakładają
            nticks: 10, // Alternatywa dla tickangle
            // showspikes: true,      // Włącz linie "spike" dla tej osi
            // spikemode: 'toaxis+across', // Linia do osi i przez inne panele
            // spikesnap: 'cursor',   // Przyciągaj linię do kursora
            // spikedash: 'dot',    // Styl linii
            // spikethickness: 1,
            // spikecolor: '#888',    // Kolor linii
        },
        yaxis: { // Oś ceny
            ...styleConfig.defaultAxisLayout,
            title: 'Cena',
            domain: [0.25, 1], // Górne 75%
            autorange: true,
			// showspikes: true,
			// spikemode: 'toaxis', // Linia tylko do osi Y
			// spikesnap: 'cursor',
			// spikedash: 'dot',
			// spikethickness: 1,
			// spikecolor: '#888',
        },
        yaxis2: { // Oś wolumenu
            ...styleConfig.defaultAxisLayout,
            title: 'Wolumen',
            domain: [0, 0.2], // Dolne 20%
            showticklabels: true,
            side: 'left', // Przeniosłem na lewo dla czytelności z ceną
            autorange: true,
            // Można dodać formatowanie liczb dla wolumenu, jeśli potrzebne
             // tickformat: ',.0f' // Np. separatory tysięcy
			// showspikes: true,
			// spikemode: 'toaxis',
			// spikesnap: 'cursor',
			// spikedash: 'dot',
			// spikethickness: 1,
			// spikecolor: '#888',
        },
        hovermode: 'x unified',
        dragmode: 'zoom',
        legend: {
            x: 0,
            y: 1.1,
            orientation: "h"
        },
        margin: { l: 60, r: 30, t: 50, b: 70 } // Dopasuj marginesy, aby tytuły osi się mieściły
    };
}

/**
 * Generuje definicję śladu (trace) dla ceny (świecowy lub liniowy).
 * @param {'candlestick' | 'scatter'} type Typ śladu.
 * @param {Array<string>} dates Tablica dat (stringi YYYY-MM-DD).
 * @param {Array<number|null>} opens Tablica cen otwarcia.
 * @param {Array<number|null>} highs Tablica cen maksymalnych.
 * @param {Array<number|null>} lows Tablica cen minimalnych.
 * @param {Array<number|null>} closes Tablica cen zamknięcia.
 * @param {object | null} stockInfo Informacje o spółce (dla nazwy śladu).
 * @returns {object} Obiekt śladu Plotly.
 */
function getPriceTrace(type, dates, opens, highs, lows, closes, stockInfo) {
    const baseTrace = {
        x: dates,
        name: stockInfo?.symbol || 'Cena',
        yaxis: 'y' // Odnosi się do yaxis (głównej)
    };

    if (type === 'candlestick') {
        return {
            ...baseTrace, // Rozszerz bazową konfigurację
            type: 'candlestick',
            open: opens,
            high: highs,
            low: lows,
            close: closes,
            increasing: {
                line: { color: styleConfig.candleBorderColor, width: styleConfig.candleBorderWidth },
                fillcolor: styleConfig.candleIncreasingColor
            },
            decreasing: {
                line: { color: styleConfig.candleBorderColor, width: styleConfig.candleBorderWidth },
                fillcolor: styleConfig.candleDecreasingColor
            }
        };
    } else if (type === 'scatter') {
        return {
            ...baseTrace,
            type: 'scatter',
            y: closes, // Linia na podstawie cen zamknięcia
            mode: 'lines',
            line: { color: styleConfig.lineChartColor }
        };
    }
    throw new Error(`Nieznany typ śladu ceny: ${type}`);
}

/**
 * Generuje definicję śladu (trace) dla wolumenu.
 * @param {Array<string>} dates Tablica dat (stringi YYYY-MM-DD).
 * @param {Array<number|null>} volumes Tablica wolumenów.
 * @returns {object} Obiekt śladu Plotly.
 */
function getVolumeTrace(dates, volumes) {
    return {
        x: dates,
        y: volumes,
        type: 'bar',
        name: 'Wolumen',
        yaxis: 'y2', // Odnosi się do yaxis2
        marker: { color: styleConfig.volumeColor },
	};
}

/**
 * Generuje definicję śladu dla SMA.
 * @param {Array<string>} dates Tablica dat.
 * @param {Array<number|null>} smaValues Tablica wartości SMA.
 * @param {number} period Okres SMA.
 * @returns {object} Obiekt śladu Plotly.
 */
function getSmaTrace(dates, smaValues, period) {
    return {
        x: dates,
        y: smaValues,
        type: 'scatter', // Linia
        mode: 'lines',
        name: `SMA(${period})`,
        yaxis: 'y', // Na głównej osi ceny
        line: {
            color: 'orange', // Kolor SMA
            width: 1
        }
    };
}

/**
 * Generuje definicje śladów dla Kanału Cenowego (górna i dolna linia).
 * @param {Array<string>} dates Tablica dat.
 * @param {Array<number|null>} upperValues Tablica górnych wartości kanału.
 * @param {Array<number|null>} lowerValues Tablica dolnych wartości kanału.
 * @param {number} period Okres kanału.
 * @returns {Array<object>} Tablica obiektów śladów Plotly (górny i dolny).
 */
function getPriceChannelTraces(dates, upperValues, lowerValues, period) {
    const common = {
        x: dates,
        type: 'scatter',
        mode: 'lines',
        yaxis: 'y', // Na głównej osi ceny
        line: {
            color: 'purple', // Kolor kanału
            width: 1,
            dash: 'dot' // Styl linii (np. kropkowana)
        }
    };
    return [
        { ...common, y: upperValues, name: `Kanał Górny(${period})` },
        { ...common, y: lowerValues, name: `Kanał Dolny(${period})` }
    ];
}

/**
 * Generuje definicje śladów dla Wstęg Bollingera.
 * @param {Array<string>} dates Tablica dat.
 * @param {object} bbData Obiekt z tablicami { middle, upper, lower }.
 * @param {number} period Okres BB.
 * @param {number} stdDev Odchylenie standardowe BB.
 * @returns {Array<object>} Tablica obiektów śladów Plotly (np. 3 linie).
 */
function getBollingerBandsTraces(dates, bbData, period, stdDev) {
    // Można rysować 3 linie lub obszar + linię środkową
    // Poniżej wersja z 3 liniami:
    const commonLine = {
         x: dates,
         type: 'scatter',
         mode: 'lines',
         yaxis: 'y', // Na głównej osi ceny
         line: { width: 1 }
    };
    return [
        { // Linia górna
            ...commonLine,
            y: bbData.upper,
            name: `BB Górna(${period},${stdDev})`,
            line: { ...commonLine.line, color: 'gray', dash: 'dash' }
        },
         { // Linia środkowa (SMA)
            ...commonLine,
            y: bbData.middle,
            name: `BB Środek(${period})`,
            line: { ...commonLine.line, color: 'darkgray' } // Ciemniejszy szary dla środka
        },
        { // Linia dolna
            ...commonLine,
            y: bbData.lower,
            name: `BB Dolna(${period},${stdDev})`,
            line: { ...commonLine.line, color: 'gray', dash: 'dash' }
        }
        // Alternatywnie można by narysować obszar między górną a dolną:
        // { x: dates, y: bbData.upper, ..., showlegend: false, line: { color: 'rgba(128,128,128,0.2)' } },
        // { x: dates, y: bbData.lower, ..., fill: 'tonexty', fillcolor: 'rgba(128,128,128,0.2)', showlegend: false, line: { color: 'rgba(128,128,128,0.2)' } },
        // { x: dates, y: bbData.middle, ... } // Tylko linia środkowa
    ];
}

/**
 * Generuje definicję śladu dla ATR.
 * @param {Array<string>} dates Tablica dat.
 * @param {Array<number|null>} atrValues Tablica wartości ATR.
 * @param {number} period Okres ATR.
 * @param {string} yaxisId ID osi Y, na której ma być narysowany ATR (np. 'y3').
 * @returns {object} Obiekt śladu Plotly.
 */
function getAtrTrace(dates, atrValues, period, yaxisId) {
    return {
        x: dates,
        y: atrValues,
        type: 'scatter',
        mode: 'lines',
        name: `ATR(${period})`,
        yaxis: yaxisId, // Użyj podanej osi Y
        line: {
            color: 'teal', // Kolor ATR
            width: 1.5
        }
    };
}


// Eksportujemy WSZYSTKIE funkcje
export {
    getOhlcvLayout, getPriceTrace, getVolumeTrace,
    getSmaTrace, getPriceChannelTraces, getBollingerBandsTraces, getAtrTrace
};
