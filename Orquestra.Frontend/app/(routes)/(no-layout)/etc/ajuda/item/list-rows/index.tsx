import { iAjudaItem, iAjudaTopico } from '@/app/(routes)/(no-layout)/etc/ajuda/page';
import ImgServer from '@/app/assets/png/server.png';
import CardSimple from '@/app/components/card/simple';
import Seta from '@/app/components/svg/seta/seta';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import handleNormalizeUrl from '@/app/functions/format.url';
import { useRouter } from 'next/navigation';
import styles from './index.module.scss';

interface iParametros {
    filteredTopic: iAjudaTopico | undefined;
}

export default function AjudaListRows({ filteredTopic }: iParametros) {

    const router = useRouter();

    return (
        <div className={styles.main}>
            {
                filteredTopic && filteredTopic?.items ? (
                    filteredTopic?.items.map((item: iAjudaItem, i: number) => (
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
                        <CardSimple
                            img={ImgServer}
                            title='Houston, temos um problema'
                            description='Acho que agora você realmente precisa de ajuda... porque que nenhuma ajuda foi encontrada por aqui.'
                            buttonLabel='Voltar à central de ajuda'
                            buttonFunction={() => router.push(ROUTES.ETC_AJUDA)}
                        />
                    </div>
                )
            }
        </div>
    )
}