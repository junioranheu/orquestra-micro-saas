import { iAjudaTopicoItem } from '@/app/(routes)/(no-layout)/(etc)/ajuda/page';
import Seta from '@/app/components/svg/seta/seta';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import handleNormalizeUrl from '@/app/functions/format.url';
import { useRouter } from 'next/navigation';
import styles from './index.module.scss';

interface iProps {
    filteredTopicItems: iAjudaTopicoItem[] | undefined;
}

export default function AjudaListRows({ filteredTopicItems }: iProps) {

    const router = useRouter();

    return (
        <div className={styles.main}>
            {
                filteredTopicItems && filteredTopicItems?.length ? (
                    filteredTopicItems?.map((item: iAjudaTopicoItem, i: number) => (
                        <div
                            key={i}
                            className={styles.item}
                            onClick={() => router.push(`${ROUTES.ETC_AJUDA}/item/${handleNormalizeUrl(item?.title)}`)}
                        >
                            <div className={styles.itemInner}>
                                <span className='pointer'>
                                    {item?.title}
                                </span>

                                <Seta />
                            </div>
                        </div>
                    ))
                ) : (
                    <div className={`${styles.notFound} ${SYSTEM.ANIMATE_DELAY_0_5s}`}>
                        <span>Opa! Nenhum tópico foi encontrado.</span>
                    </div>
                )
            }
        </div>
    )
}