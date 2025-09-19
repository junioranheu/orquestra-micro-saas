import { iAjudaItem, iAjudaTopico } from '@/app/(routes)/(no-layout)/etc/ajuda/page';
import Seta from '@/app/components/svg/seta/seta';
import handleNormalizeUrl from '@/app/functions/format.url';
import Router from 'next/router';
import styles from './index.module.scss';

interface iParametros {
    filteredTopic: iAjudaTopico | undefined;
    query: string;
    showTopic: boolean;
}

export default function AjudaListRows({ filteredTopic, query, showTopic }: iParametros) {

    return (
        <div className={styles.main}>
            {
                filteredTopic?.items.map((item: iAjudaItem, i: number) => (
                    <div
                        key={i}
                        className={styles.item}
                        onClick={() => Router.push(`/ajuda/item/${handleNormalizeUrl(item?.title)}`)}
                    >
                        <div className={styles.itemInner}>
                            <span className='pointer'>
                                {item?.title}

                                {
                                    // showTopic && (
                                    //     <span> / <span dangerouslySetInnerHTML={{ __html: item?.ajudasTopicos?.topico }} /></span>
                                    // )
                                }
                            </span>

                            <Seta />
                        </div>
                    </div>
                ))
            }
        </div>
    )
}

