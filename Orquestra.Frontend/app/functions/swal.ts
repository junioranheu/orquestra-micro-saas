import Swal, { SweetAlertResult } from 'sweetalert2';

interface iProps {
    title?: string;
    content?: string;
    cancelBtnText?: string;
    cancelFunction?: () => void;
    confirmBtnText?: string;
    confirmFunction?: () => void;
    allowOutsideClick?: boolean;
    mustConfirm?: boolean;
    checkboxLabel?: string;
    icon?: undefined | 'info' | 'success' | 'error' | 'warning' | 'question';
}

export default function swal(options: iProps): Promise<any> {
    return new Promise((resolve) => {
        const { title, content, cancelBtnText, cancelFunction, confirmBtnText = 'Ok', confirmFunction, allowOutsideClick = false, mustConfirm = false, checkboxLabel = 'Concordo', icon = undefined } = options;

        Swal.fire({
            title: title,
            html: content?.toString()?.replace(/^Error:\s*/, ''),
            icon: icon,
            allowOutsideClick: mustConfirm ? false : allowOutsideClick,
            allowEscapeKey: !mustConfirm,
            reverseButtons: true,
            showConfirmButton: (confirmBtnText?.length ?? 0) > 0,
            confirmButtonText: confirmBtnText,
            showCancelButton: (cancelBtnText?.length ?? 0) > 0,
            cancelButtonText: cancelBtnText,
            input: mustConfirm ? 'checkbox' : undefined,
            inputPlaceholder: mustConfirm ? checkboxLabel : undefined,
            inputValidator: mustConfirm ? (value: any) => {
                if (!value) {
                    return 'Você precisa marcar a caixinha acima para continuar!';
                }

                return null;
            } : undefined,
            didOpen: () => {
                const htmlContainer = Swal.getHtmlContainer();

                if (!htmlContainer) {
                    return;
                }

                const text = htmlContainer.textContent?.trim() ?? '';
                const isLong = text?.length > 70;

                htmlContainer.classList.add(isLong ? 'swal2-to-left' : 'swal2-to-center');
            }
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