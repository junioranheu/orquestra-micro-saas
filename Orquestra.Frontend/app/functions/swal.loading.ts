import Swal from 'sweetalert2';

interface iProps {
    message?: string;
    handleFunction?: () => void;
    timeoutMs?: number;
}

export default function swalLoading({ message = '<p>Atualizando os dados...</p>', handleFunction, timeoutMs = 1000 }: iProps): Promise<any> {
    return new Promise((resolve) => {
        Swal.fire({
            html: message,
            allowOutsideClick: false,
            didOpen: () => {
                Swal.showLoading();

                setTimeout(() => {
                    Swal.close();

                    if (handleFunction) {
                        handleFunction();
                    }

                    resolve(true);
                }, timeoutMs);
            }
        });
    });

}