import { CONSTS_AUTH } from '@/app/api/consts/auth';
import { Fetch } from '@/app/api/fetch';
import swal from '@/app/functions/swal';
import { useEffect, useState } from 'react';
import Swal from 'sweetalert2';

export default function useCheckAzureServer() {

    const [loading, setLoading] = useState<boolean>(true);
    const thresholdInMsToStartChecking = 2500;

    useEffect(() => {
        let swalServer: any;

        // async function fetchGetFake() {
        //     return new Promise((resolve) => {
        //         setTimeout(() => {
        //             resolve('mock_getFakefake');
        //         }, thresholdInMsToStartChecking + 5000);
        //     });
        // }

        async function handleCheck() {
            try {
                // await fetchGetFake();
                await Fetch.get({ url: CONSTS_AUTH.meSimple });
            } catch (error: unknown) {
                const errorMsg = error instanceof Error ? error.message : String(error);
                swal({ content: errorMsg });
            } finally {
                setLoading(false);

                if (swalServer) {
                    swalServer.close();
                }
            }
        }

        const timeout = setTimeout(() => {
            if (loading) {
                swalServer = Swal.fire({
                    html: '<p>Por favor, aguarde alguns instantes enquanto o servidor é iniciado.</p>',
                    allowOutsideClick: false,
                    didOpen: () => {
                        Swal.showLoading();
                    }
                });

                setLoading(false);
            }
        }, thresholdInMsToStartChecking);

        if (loading) {
            handleCheck();
        }

        return () => clearTimeout(timeout);
    }, [loading]);

}