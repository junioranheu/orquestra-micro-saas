import { DATE_STYLE, handleFormatDate } from './format.date';
import handleGetDateBrazil from './get.date.brazil';

interface iProps {
    mustIncludeUmUma: boolean;
}

export function handleGetGreetingDayInfo({ mustIncludeUmUma }: iProps): string {
    const date = handleGetDateBrazil();
    const dayOfWeek = date.getDay(); // 0 = domingo, 1 = segunda, ..., 6 = sábado;
    const dateStr = handleFormatDate(date, DATE_STYLE.DIA_DA_SEMANA_E_DIA_DO_MES);

    if (mustIncludeUmUma) {
        const greeting = dayOfWeek >= 1 && dayOfWeek <= 5 ? 'Uma boa' : 'Um bom';
        const output = `${greeting} ${dateStr}`;

        return output;
    }

    const greeting = dayOfWeek >= 1 && dayOfWeek <= 5 ? 'Boa' : 'Bom';
    const output = `${greeting} ${dateStr}`;

    return output;
}