import Router from 'next/router';
import { useEffect, useState } from 'react';
import SetaDois from '../../../../components/svg/seta.dois';
import { Fetch } from '../../../../utils/api/fetch';
import CONSTS_AJUDAS_ITENS from '../../../../utils/consts/data/constAjudasItens';
import CONSTS_AJUDAS_TOPICOS from '../../../../utils/consts/data/constAjudasTopicos';
import CONSTS_SISTEMA from '../../../../utils/consts/outros/sistema';
import CONSTS_TELAS from '../../../../utils/consts/outros/telas';
import ajustarUrl from '../../../../utils/outros/ajustarUrl';
import paginaCarregada from '../../../../utils/outros/paginaCarregada';
import removerHTML from '../../../../utils/outros/removerHTML';
import iAjudaItem from '../../../../utils/types/ajuda.item';
import iAjudaTopico from '../../../../utils/types/ajuda.topico';
import AjudaListaItens from '../../item/outros/ajuda.listaItens';
import Styles from './topico.module.scss';

interface iParametros {
    listaAjudasItens: iAjudaItem[];
}

export default function Topico({ listaAjudasItens }: iParametros) {

    document.title = `${(listaAjudasItens[0]?.ajudasTopicos?.topico ? removerHTML(listaAjudasItens[0]?.ajudasTopicos?.topico) : 'Ajuda')} — ${CONSTS_SISTEMA.NOME_SISTEMA}`;

    const [isLoaded, setIsLoaded] = useState<boolean>(false);
    useEffect(() => {
        paginaCarregada(true, 300, 600, setIsLoaded);
    }, [listaAjudasItens]);

    if (!isLoaded) {
        return false;
    }

    return (
        <section className={`${Styles.main} paddingPadrao`}>
            <div className={Styles.divVoltar} onClick={() => Router.push(CONSTS_TELAS.AJUDA)}>
                <SetaDois width={16} url={null} title='Voltar' isCorPrincipal={true} />
                <span className='texto pointer cor-principal-hover'>Voltar à central de ajuda</span>
            </div>

            <div className='margem3'>
                <div className={Styles.titulo} dangerouslySetInnerHTML={{ __html: (listaAjudasItens[0]?.ajudasTopicos?.topico ?? '') }} />
            </div>

            {/* Lista de itens de ajuda do tópico em questão */}
            <AjudaListaItens
                listaAjudasItens={listaAjudasItens}
                queryBuscada=''
                isMargemTop={true}
                isExibirTopico={false}
            />

            {/* Espaço a mais */}
            <div className='espacoBottom'></div>
        </section>
    )
}

export async function getStaticPaths() {
    // Todos os tópicos;
    const url = CONSTS_AJUDAS_TOPICOS.API_URL_GET_TODOS;
    const topicos = await Fetch.getApi(url) as iAjudaTopico[];
    // console.log(topicos);

    // Gerar o "paths";
    const paths = topicos?.map((i: iAjudaTopico) => ({
        params: {
            ajudaTopicoId: i.ajudaTopicoId.toString(),
            topico: ajustarUrl(removerHTML(i.topico))
        }
    }));

    return {
        paths,
        fallback: false
    }
}

export async function getStaticProps(context: any) {
    const id = context.params.ajudaTopicoId;

    // Itens do tópico;
    const url = `${CONSTS_AJUDAS_ITENS.API_URL_GET_BY_AJUDA_TOPICO_ID}/${id}`;
    const listaAjudasItens = await Fetch.getApi(url) as iAjudaItem[];
    // console.log(listaAjudasItens);

    return {
        props: {
            listaAjudasItens
        }
    }
}