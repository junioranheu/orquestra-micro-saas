import { Fetch } from '@/app/api/fetch';
import { Dispatch, SetStateAction } from 'react';
import { DATE_STYLE, handleFormatDate } from './format.date';
import handleGetDateBrazil from './get.date.brazil';
import swal from './swal';

interface iParams {
    setIsRequestLoading: Dispatch<SetStateAction<boolean>>;
    fileName: string;
    apiUrl: string;
    messageSuccess: string;
    messageError: string;
}

export default async function handleFileCSVExport(params: iParams): Promise<boolean> {
    const { setIsRequestLoading, fileName, apiUrl, messageSuccess, messageError } = params;

    try {
        setIsRequestLoading(true);
        const csv = `${fileName}-${handleFormatDate(handleGetDateBrazil(), DATE_STYLE.DIA_MES_ANO_HORA_MINUTO_SEGUNDO)}.csv`;
        await Fetch.get(apiUrl, '', csv);
    } catch (error: unknown) {
        setIsRequestLoading(false);

        await swal({
            str: messageError,
            icon: 'error'
        });

        return false;
    }

    setIsRequestLoading(false);

    await swal({
        str: messageSuccess,
        icon: 'success'
    });

    return true;
}