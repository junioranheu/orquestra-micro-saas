import { iAjudaTopicoItem } from '@/app/(routes)/(no-layout)/(etc)/ajuda/page';
import Seta from '@/app/components/svg/seta/seta';
import SYSTEM from '@/app/consts/system';
import swal from '@/app/functions/swal';
import styles from './index.module.scss';

interface iProps {
    filteredTopicItems: iAjudaTopicoItem[] | undefined;
}

export default function AjudaListRows({ filteredTopicItems }: iProps) {

    function handleOpenSwal(item: iAjudaTopicoItem) {
        swal({
            title: item.title,
            str: item.description,
            confirmBtnText: 'Entendi',
            icon: 'info'
        });
    }

    return (
        <div className={styles.main}>
            {
                filteredTopicItems && filteredTopicItems?.length ? (
                    filteredTopicItems?.map((item: iAjudaTopicoItem, i: number) => (
                        <div
                            key={i}
                            className={styles.item}
                            // onClick={() => router.push(`${ROUTES.ETC_AJUDA}/item/${handleNormalizeUrl(item?.title)}`)}
                            onClick={() => handleOpenSwal(item)}
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