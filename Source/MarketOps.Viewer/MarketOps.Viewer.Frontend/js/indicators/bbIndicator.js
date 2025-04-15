// js/indicators/bbIndicator.js
import { getBollingerBandsTraces } from '../chartStyleConfig.js';
import { padBBDataForPlotly } from './indicatorUtils.js';

if (typeof bollingerbands !== 'function') {
     console.error("Funkcja 'bollingerbands' z technicalindicators niedostępna!");
}

const bbIndicator = {
    /**
     * Oblicza Bollinger Bands.
     * @param {object} inputData Obiekt z tablicami { close, ... }
     * @param {number} period Okres BB.
     * @param {number} stdDev Odchylenie standardowe.
     * @returns {Array<object>|null} Tablica obiektów { middle, upper, lower } lub null.
     */
    calculate(inputData, period, stdDev) {
        if (typeof bollingerbands !== 'function' || !inputData || !inputData.close || inputData.close.length < period) {
            return null;
        }
         try {
             // Zwraca krótszą tablicę obiektów
             const result = bollingerbands({ period: period, values: inputData.close, stdDev: stdDev });
             const expectedLength = inputData.close.length - (period - 1);
             if (result && result.length === expectedLength) {
                 return result;
             } else {
                  console.warn(`BB calculate: Oczekiwano ${expectedLength} wyników, otrzymano ${result?.length}`);
                  return null;
             }
        } catch (error) {
            console.error("Błąd podczas obliczania BB:", error);
            return null;
        }
    },

    /**
     * Generuje ślady Plotly dla Bollinger Bands.
     * @param {Array<string>} allDates Pełna tablica dat.
     * @param {Array<object>} indicatorResult Wynik z metody calculate (krótszy).
     * @param {object} state Stan wskaźnika { period, stdDev, ... }.
     * @returns {Array<object>} Tablica obiektów śladów Plotly.
     */
    getTraces(allDates, indicatorResult, indicatorConfig) {
        const dataLength = allDates.length;
        const period = indicatorConfig.period;
		const stdDev = indicatorConfig.stdDev;
        const valuesForPlotly = padBBDataForPlotly(indicatorResult, dataLength, period);

        if (valuesForPlotly) {
            return getBollingerBandsTraces(allDates, valuesForPlotly, period, stdDev);
        } else {
            return [];
        }
    }
};

export { bbIndicator };