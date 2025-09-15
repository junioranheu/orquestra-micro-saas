'use client';
import ImgLoading from '@/app/assets/gif/loading.gif';
import toast from '@/app/functions/toast';
import { useIsRequestLoading } from '@/app/hooks/contexts/useGlobalContext';
import useDisableScroll from '@/app/hooks/useDisableScroll';
import Image from 'next/image';
import { useEffect } from 'react';
import { toast as ToastSonner } from 'sonner';
import styles from './index.module.scss';

interface iProps {
    typeMessage?: 'normal' | 'long';
}

export default function Loading({ typeMessage = 'normal' }: iProps) {

    const [isRequestLoading, setIsRequestLoading] = useIsRequestLoading();
    useDisableScroll(isRequestLoading);

    useEffect(() => {
        if (isRequestLoading) {
            const timeoutDuration = typeMessage === 'long' ? (5 * 60 * 1000) : (3 * 1000);
            const message = typeMessage === 'long' ? 'A sua requisição está em progresso. Isso pode demorar...' : 'A sua requisição está em progresso. Aguarde uns instantes.';

            toast({ content: message, isClosable: false });

            const timer = setTimeout(() => {
                ToastSonner.dismiss();
                setIsRequestLoading(false);
            }, timeoutDuration);

            return () => clearTimeout(timer);
        }
    }, [isRequestLoading, typeMessage, setIsRequestLoading]);

    if (!isRequestLoading) {
        setTimeout(() => {
            ToastSonner.dismiss();
        }, 3000);

        return;
    }

    return (
        <section className={styles.main}>
            <div className={styles.loading}>
                <Image src={ImgLoading} alt={'Carregando'} width={50} priority={true} unoptimized={true} />
            </div>
        </section>
    )
}