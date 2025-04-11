// js/indicators/indicatorUtils.js

/**
 * Przygotowuje dane dla Plotly, dodając null padding na początku.
 * @param {Array<any>} fullIndicatorData Pełna tablica wyników z biblioteki (może mieć undefined/NaN).
 * @param {number} dataLength Oczekiwana pełna długość danych (jak allDates).
 * @param {number} period Okres wskaźnika (lub validStartIndex).
 * @param {boolean} [mapUndefinedToNull=false] Czy jawnie mapować undefined/NaN na null.
 * @returns {Array<any>|null} Tablica gotowa dla Plotly lub null w przypadku błędu.
 */
function padDataForPlotly(fullIndicatorData, dataLength, period, mapUndefinedToNull = false) {
    if (!fullIndicatorData) return null;

    const validStartIndex = period - 1; // Domyślnie dla większości
    // Można dodać logikę dla ATR jeśli validStartIndex jest inny
    // if (indicatorType === 'ATR') validStartIndex = period;

    const nullPaddingCount = validStartIndex;
    const expectedResultLength = dataLength - nullPaddingCount;

    console.log(`Padding - Data L: ${dataLength}, Period: ${period}, Expected Result L: ${expectedResultLength}, Actual Result L: ${fullIndicatorData.length}`); // DEBUG

    if (fullIndicatorData.length !== expectedResultLength) {
        console.warn(`Niezgodna długość wyniku z biblioteki (${fullIndicatorData.length} vs ${expectedResultLength} oczekiwano).`);
        return null; // Zwróć null, aby zasygnalizować błąd
    }

    let paddedData;
    if (mapUndefinedToNull) {
        // Stwórz tablicę z nullami na początku i resztą zamienioną na null jeśli undefined/NaN
        paddedData = Array(nullPaddingCount).fill(null).concat(
            fullIndicatorData.map(val => (val === undefined || isNaN(val)) ? null : val)
        );
    } else {
        // Stwórz tablicę z nullami na początku i oryginalnymi wartościami (z undefined/NaN)
        paddedData = Array(nullPaddingCount).fill(null).concat(fullIndicatorData);
    }

    // Ostateczne sprawdzenie długości
    if (paddedData.length !== dataLength) {
        console.error("Niezgodność długości po paddingu!");
        return null;
    }

    return paddedData;
}


/**
 * Przygotowuje dane dla Bollinger Bands z paddingiem.
 */
function padBBDataForPlotly(fullIndicatorData, dataLength, period) {
    if (!fullIndicatorData) return null;

    const validStartIndex = period - 1;
    const nullPaddingCount = validStartIndex;
    const expectedResultLength = dataLength - nullPaddingCount;

     console.log(`Padding BB - Data L: ${dataLength}, Period: ${period}, Expected Result L: ${expectedResultLength}, Actual Result L: ${fullIndicatorData.length}`); // DEBUG

    if (fullIndicatorData.length !== expectedResultLength) {
        console.warn(`BB - Niezgodna długość wyniku z biblioteki (${fullIndicatorData.length} vs ${expectedResultLength} oczekiwano).`);
        return null;
    }

    const valuesForPlotly = {
        middle: Array(nullPaddingCount).fill(null).concat(fullIndicatorData.map(d => d?.middle)),
        upper: Array(nullPaddingCount).fill(null).concat(fullIndicatorData.map(d => d?.upper)),
        lower: Array(nullPaddingCount).fill(null).concat(fullIndicatorData.map(d => d?.lower))
     };

    if (valuesForPlotly.middle.length !== dataLength) {
        console.error("BB - Niezgodność długości po paddingu!");
        return null;
    }
    return valuesForPlotly;
}

export { padDataForPlotly, padBBDataForPlotly };