import ROUTES from '@/app/consts/routes';
import Swal, { SweetAlertResult } from 'sweetalert2';

export default function swalUnauthorized(msg: string): Promise<any> {
    return new Promise((resolve) => {
        const finalMsg = msg ?? 'Sua sessão expirou ou você não tem acesso para realizar esta ação.';

        Swal.fire({
            title: 'Não autorizado',
            html: finalMsg,
            icon: 'warning',
            allowOutsideClick: false,
            reverseButtons: true,
            showConfirmButton: true,
            confirmButtonText: 'Finalizar sessão',
            showCancelButton: false,
            cancelButtonText: 'Voltar'
        }).then((result: SweetAlertResult) => {
            if (result.isConfirmed) {
                resolve(handleLogout());
            }
            else {
                // resolve(true);
                resolve(handleLogout());
            }
        });
    });
}

function handleLogout(): boolean {
    window.location.href = ROUTES.LOGOUT;
    return true;
}