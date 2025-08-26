import ImgLoading from '@/app/assets/gif/loading.gif';
import { useIsRequestLoading } from '@/app/hooks/contexts/useGlobalContext';
import useDisableScroll from '@/app/hooks/useDisableScroll';
import Image from 'next/image';
import { useEffect } from 'react';
import styles from './loading.module.scss';

interface iParams {
    typeMessage?: 'normal' | 'long';
}

export default function Loading({ typeMessage = 'normal' }: iParams) {

    const [isRequestLoading, setIsRequestLoading] = useIsRequestLoading();
    useDisableScroll(isRequestLoading);

    useEffect(() => {
        if (isRequestLoading) {
            const timeoutDuration = typeMessage === 'long' ? (5 * 60 * 1000) : (15 * 1000);

            const timer = setTimeout(() => {
                setIsRequestLoading(false);
            }, timeoutDuration);

            return () => clearTimeout(timer);
        }
    }, [isRequestLoading, typeMessage, setIsRequestLoading]);

    if (!isRequestLoading) {
        return;
    }

    return (
        <section className={styles.main}>
            <div className={styles.loading}>
                <Image src={ImgLoading} alt={'Carregando'} width={50} priority={true} unoptimized={true} />

                {
                    typeMessage === 'long' ? <span>A sua requisição está em progresso. Isso pode demorar...</span> : <span>A sua requisição está em progresso. Aguarde uns instantes.</span>
                }
            </div>
        </section>
    )
}