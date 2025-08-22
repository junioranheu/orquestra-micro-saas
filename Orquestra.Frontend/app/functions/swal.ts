import Swal, { SweetAlertResult } from 'sweetalert2';

interface iParams {
    title?: string;
    str?: string;
    cancelBtnText?: string;
    cancelFunction?: () => void;
    confirmBtnText?: string;
    confirmFunction?: () => void;
    allowOutsideClick?: boolean;
    icon?: undefined | 'info' | 'success' | 'error' | 'warning' | 'question';
}

export default function swal(options: iParams): Promise<any> {
    return new Promise((resolve) => {
        const { title, str, cancelBtnText, cancelFunction, confirmBtnText = 'Ok', confirmFunction, allowOutsideClick = true, icon = undefined } = options;

        Swal.fire({
            title: title,
            html: str?.toString()?.replace(/^Error:\s*/, ''),
            icon: icon,
            allowOutsideClick: allowOutsideClick,
            reverseButtons: true,
            showConfirmButton: confirmBtnText?.length! > 0,
            confirmButtonText: confirmBtnText,
            showCancelButton: cancelBtnText?.length! > 0,
            cancelButtonText: cancelBtnText
        }).then((result: SweetAlertResult) => {
            if (result.isConfirmed && confirmFunction) {
                // User clicked confirm button;
                resolve(confirmFunction());
            } else if (result.isDismissed && result.dismiss === Swal.DismissReason.cancel && cancelFunction) {
                // User clicked cancel button;
                resolve(cancelFunction());
            } else {
                // User clicked outside the modal;
                resolve(undefined);
            }
        });
    });
}