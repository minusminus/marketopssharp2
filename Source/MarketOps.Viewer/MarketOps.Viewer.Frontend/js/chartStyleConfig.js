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
            //tickangle: -45 // Można dodać, jeśli etykiety się nakładają
            nticks: 10 // Alternatywa dla tickangle
        },
        yaxis: { // Oś ceny
            ...styleConfig.defaultAxisLayout,
            title: 'Cena',
            domain: [0.25, 1], // Górne 75%
            autorange: true
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
        marker: { color: styleConfig.volumeColor }
    };
}

// Eksportujemy funkcje, aby mogły być używane przez chartService
export { getOhlcvLayout, getPriceTrace, getVolumeTrace };
