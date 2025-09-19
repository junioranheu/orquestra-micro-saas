import Router from 'next/router';
import { useEffect, useState } from 'react';
import SetaDois from '../../../../components/svg/seta.dois';
import { Fetch } from '../../../../utils/api/fetch';
import CONSTS_AJUDAS_ITENS from '../../../../utils/consts/data/constAjudasItens';
import CONSTS_SISTEMA from '../../../../utils/consts/outros/sistema';
import ajustarUrl from '../../../../utils/outros/ajustarUrl';
import paginaCarregada from '../../../../utils/outros/paginaCarregada';
import removerHTML from '../../../../utils/outros/removerHTML';
import iAjudaItem from '../../../../utils/types/ajuda.item';
import Styles from './item.module.scss';

interface iParametros {
    ajudaItem: iAjudaItem;
}

export default function ItemAjuda({ ajudaItem }: iParametros) {

    document.title = `${(ajudaItem?.titulo ?? 'Ajuda')} — ${CONSTS_SISTEMA.NOME_SISTEMA}`;

    const [isLoaded, setIsLoaded] = useState<boolean>(false);
    useEffect(() => {
        paginaCarregada(true, 300, 600, setIsLoaded);
    }, [ajudaItem]);

    if (!isLoaded) {
        return false;
    }

    return (
        <section className={`${Styles.main} paddingPadrao`}>
            <div className={Styles.divVoltar} onClick={() => Router.push(`/ajuda/topico/${ajudaItem?.ajudaTopicoId}/${ajustarUrl(removerHTML(ajudaItem?.ajudasTopicos?.topico))}`)}>
                <SetaDois width={16} url={null} title='Voltar' isCorPrincipal={true} />
                <span className='texto pointer cor-principal-hover' title={removerHTML(ajudaItem?.ajudasTopicos?.topico)}>Voltar ao tópico original</span>
            </div>

            <div className={`${Styles.divTitulo} margem3`}>
                <span className={Styles.textoPequeno}>Central de ajuda / {removerHTML(ajudaItem?.ajudasTopicos?.topico)}</span>
                <span className={Styles.titulo}>{ajudaItem?.titulo}</span>
            </div>

            <div className='margem3'>
                <div className={Styles.conteudoHTML} dangerouslySetInnerHTML={{ __html: (ajudaItem?.conteudoHtml ?? '') }} />
            </div>

            {/* Espaço a mais */}
            <div className='espacoBottom'></div>
        </section>
    )
}

export async function getStaticPaths() {
    // Todos os itens de ajuda;
    const url = CONSTS_AJUDAS_ITENS.API_URL_GET_TODOS;
    const listaAjudasItens = await Fetch.getApi(url) as iAjudaItem[];
    // console.log(listaAjudasItens);

    // Gerar o "paths";
    const paths = listaAjudasItens?.map((i: iAjudaItem) => ({
        params: {
            ajudaItemId: i.ajudaItemId.toString(),
            titulo: ajustarUrl(i.titulo)
        }
    }));

    return {
        paths,
        fallback: false
    }
}

export async function getStaticProps(context: any) {
    const id = context.params.ajudaItemId;

    // Item (ajuda);
    const url = `${CONSTS_AJUDAS_ITENS.API_URL_GET_BY_ID}/${id}`;
    const ajudaItem = await Fetch.getApi(url) as iAjudaItem[];
    // console.log(ajudaItem);

    return {
        props: {
            ajudaItem
        }
    }
}