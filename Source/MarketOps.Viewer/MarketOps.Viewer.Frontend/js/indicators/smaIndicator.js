// js/indicators/smaIndicator.js
import { getSmaTrace } from '../chartStyleConfig.js'; // Importuj styl
import { padDataForPlotly } from './indicatorUtils.js'; // Importuj helper paddingu

// Sprawdź dostępność funkcji globalnej (jeśli jest potrzebna do obliczeń)
if (typeof sma !== 'function') {
     console.error("Funkcja 'sma' z technicalindicators niedostępna!");
}

const smaIndicator = {
    /**
     * Oblicza SMA.
     * @param {object} inputData Obiekt z tablicami { close, ... }
     * @param {number} period Okres SMA.
     * @returns {Array<number|null>|null} Tablica wyników lub null.
     */
    calculate(inputData, period) {
        if (typeof sma !== 'function' || !inputData || !inputData.close || inputData.close.length < period) {
            return null;
        }
        try {
            // Zwróć PEŁNĄ tablicę (technicalindicators zwraca krótszą dla sma)
            // Musimy ręcznie dodać undefined na początku, jeśli biblioteka tego nie robi
             const result = sma({ period: period, values: inputData.close });
             // Spodziewamy się wyniku krótszego o period - 1
             const expectedLength = inputData.close.length - (period - 1);
             if (result && result.length === expectedLength) {
                 return result; // Zwracamy krótszą, padding zrobimy w getTraces
             } else {
                  console.warn(`SMA calculate: Oczekiwano ${expectedLength} wyników, otrzymano ${result?.length}`);
                  // Można spróbować zwrócić tablicę z undefined, ale bezpieczniej zwrócić null
                  return null;
             }
        } catch (error) {
            console.error("Błąd podczas obliczania SMA:", error);
            return null;
        }
    },

    /**
     * Generuje ślady Plotly dla SMA.
     * @param {Array<string>} allDates Pełna tablica dat.
     * @param {Array<number|null>} indicatorResult Wynik z metody calculate (krótszy).
     * @param {object} state Stan wskaźnika { period, ... }.
     * @returns {Array<object>} Tablica obiektów śladów Plotly.
     */
    getTraces(allDates, indicatorResult, indicatorConfig) {
        const dataLength = allDates.length;
        const period = indicatorConfig.period;
        const valuesForPlotly = padDataForPlotly(indicatorResult, dataLength, period);

        if (valuesForPlotly) {
            return [getSmaTrace(allDates, valuesForPlotly, period)];
        } else {
            return []; // Zwróć pustą tablicę, jeśli padding zawiódł
        }
    }
};

export { smaIndicator };