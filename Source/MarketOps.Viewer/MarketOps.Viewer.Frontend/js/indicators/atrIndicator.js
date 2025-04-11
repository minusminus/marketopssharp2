// js/indicators/atrIndicator.js
import { getAtrTrace } from '../chartStyleConfig.js';
import { padDataForPlotly } from './indicatorUtils.js';

if (typeof atr !== 'function') {
     console.error("Funkcja 'atr' z technicalindicators niedostępna!");
}

const atrIndicator = {
     /**
     * Oblicza ATR.
     * @param {object} inputData Obiekt z tablicami { high, low, close, ... }
     * @param {number} period Okres ATR.
     * @returns {Array<number|null>|null} Tablica wyników lub null.
     */
    calculate(inputData, period) {
         if (typeof atr !== 'function' || !inputData || !inputData.high || inputData.high.length < period + 1) { // ATR może potrzebować period+1 danych
             return null;
         }
         try {
             const atrInput = { high: inputData.high, low: inputData.low, close: inputData.close, period: period };
             // Zwraca krótszą tablicę
             const result = atr(atrInput);
             const expectedLength = inputData.close.length - period; // Pierwszy wynik dla indeksu 'period'
             if (result && result.length === expectedLength) {
                 return result;
             } else {
                  console.warn(`ATR calculate: Oczekiwano ${expectedLength} wyników, otrzymano ${result?.length}`);
                  return null;
             }
         } catch (error) {
             console.error("Błąd podczas obliczania ATR:", error);
             return null;
         }
    },

    /**
     * Generuje ślady Plotly dla ATR.
     * @param {Array<string>} allDates Pełna tablica dat.
     * @param {Array<number|null>} indicatorResult Wynik z metody calculate (krótszy).
     * @param {object} state Stan wskaźnika { period, yaxis, ... }.
     * @returns {Array<object>} Tablica obiektów śladów Plotly.
     */
    getTraces(allDates, indicatorResult, state) {
        const dataLength = allDates.length;
        const period = state.period;
        // Dla ATR padding zaczyna się od indeksu 'period'
        const valuesForPlotly = padDataForPlotly(indicatorResult, dataLength, period + 1); // Użyj period+1 dla paddingu

        if (valuesForPlotly) {
            return [getAtrTrace(allDates, valuesForPlotly, state.period, state.yaxis)];
        } else {
            return [];
        }
    }
};

export { atrIndicator };