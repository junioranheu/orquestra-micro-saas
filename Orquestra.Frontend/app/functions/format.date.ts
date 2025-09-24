export const DATE_STYLE = {
    DIA_MES_ANO: 'DIA_MES_ANO',
    DETALHADO: 'DETALHADO',
    MES_EXTENSO_E_ANO: 'MES_EXTENSO_E_ANO',
    DIA_DA_SEMANA_E_DIA_DO_MES: 'DIA_DA_SEMANA_E_DIA_DO_MES',
    DIA_DA_SEMANA: 'DIA_DA_SEMANA',
    DIA_DA_SEMANA_E_DIA_DO_MES_E_ANO: 'DIA_DA_SEMANA_E_DIA_DO_MES_E_ANO',
    DIA_MES_ANO_HORA_MINUTO_SEGUNDO: 'DIA_MES_ANO_HORA_MINUTO_SEGUNDO',
    HORA_MINUTO: 'HORA_MINUTO'
} as const;

export function handleFormatDate(date: Date | string | undefined, style: keyof typeof DATE_STYLE): string {
    try {
        if (!date) {
            return '-';
        }

        const locale = 'pt-BR';
        const isBr = true;
        const today = 'Hoje';
        const yesterday = 'Ontem';

        let dateFormatted = '';
        const dataObj = typeof date === 'string' ? new Date(date) : date;

        switch (style) {
            case DATE_STYLE.DIA_MES_ANO:
                dateFormatted = dataObj.toLocaleDateString(locale, { day: 'numeric', month: 'numeric', year: 'numeric' });
                break;
            case DATE_STYLE.DETALHADO:
                const diferencaDias = Math.floor((Date.now() - dataObj.getTime()) / (1000 * 60 * 60 * 24));

                if (diferencaDias === 0) {
                    const isMesmoDia = dataObj.getDate() === new Date().getDate();

                    if (isMesmoDia) {
                        dateFormatted = `${today}, ${dataObj.toLocaleTimeString(locale, { hour: 'numeric', minute: 'numeric', second: 'numeric', hour12: !isBr })}`;
                    } else {
                        dataObj.setDate(dataObj.getDate() - 1);
                        dateFormatted = `${yesterday}, ${dataObj.toLocaleTimeString(locale, { hour: 'numeric', minute: 'numeric', second: 'numeric', hour12: !isBr })}`;
                    }
                } else if (diferencaDias === 1) {
                    dataObj.setDate(dataObj.getDate() - 1);
                    dateFormatted = `${yesterday}, ${dataObj.toLocaleTimeString(locale, { hour: 'numeric', minute: 'numeric', second: 'numeric', hour12: !isBr })}`;
                } else {
                    dateFormatted = dataObj.toLocaleTimeString(locale, { day: 'numeric', month: 'numeric', year: 'numeric', hour: 'numeric', minute: 'numeric', second: 'numeric', hour12: !isBr });
                }
                break;
            case DATE_STYLE.MES_EXTENSO_E_ANO:
                dateFormatted = dataObj.toLocaleDateString(locale, { month: 'long', year: 'numeric' });
                break;
            case DATE_STYLE.DIA_DA_SEMANA_E_DIA_DO_MES:
                dateFormatted = dataObj.toLocaleDateString(locale, { weekday: 'long', day: 'numeric', month: 'long' });
                break;
            case DATE_STYLE.DIA_DA_SEMANA:
                dateFormatted = dataObj.toLocaleDateString(locale, { weekday: 'long' });
                break;
            case DATE_STYLE.DIA_DA_SEMANA_E_DIA_DO_MES_E_ANO:
                dateFormatted = dataObj.toLocaleDateString(locale, { weekday: 'long', day: 'numeric', month: 'long', year: 'numeric' });
                break;
            case DATE_STYLE.DIA_MES_ANO_HORA_MINUTO_SEGUNDO:
                dateFormatted = dataObj.toLocaleDateString(locale, { day: 'numeric', month: 'numeric', year: 'numeric', hour: 'numeric', minute: 'numeric', second: 'numeric', hour12: false });
                break;
            case DATE_STYLE.HORA_MINUTO:
                dateFormatted = dataObj.toLocaleTimeString(locale, { hour: '2-digit', minute: '2-digit', hour12: false });
                break;
            default:
                dateFormatted = '-';
        }

        return dateFormatted;
    } catch {
        return '-';
    }
}