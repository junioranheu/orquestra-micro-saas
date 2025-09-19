import Image from 'next/image';
import { useRouter } from 'next/router';
import nProgress from 'nprogress';
import { useEffect, useState } from 'react';
import EmojiMedicacao from '../../../static/image/outros/emoji-meditacao.webp';
import { Fetch } from '../../../utils/api/fetch';
import CONSTS_AJUDAS_ITENS from '../../../utils/consts/data/constAjudasItens';
import CONSTS_SISTEMA from '../../../utils/consts/outros/sistema';
import paginaCarregada from '../../../utils/outros/paginaCarregada';
import iAjudaItem from '../../../utils/types/ajuda.item';
import Styles from '../index.module.scss';
import AjudaListaItens from '../item/list-rows';
import AjudaInputPesquisaTopico from '../seach-input';

export default function BuscaAjuda() {

    document.title = `Ajuda — ${CONSTS_SISTEMA.NOME_SISTEMA}`;
    const router = useRouter();

    const [queryBuscada, setQueryBuscada] = useState<string>('');
    const [listaAjudasItens, setListaAjudasItens] = useState<iAjudaItem[]>();
    const [isLoaded, setIsLoaded] = useState<boolean>(false);
    useEffect(() => {
        async function handleBuscar(query: string) {
            if (!query) {
                return false;
            }

            nProgress.start();
            const url = `${CONSTS_AJUDAS_ITENS.API_URL_GET_BY_QUERY}/${query}`;
            const resposta = await Fetch.getApi(url) as iAjudaItem[];
            // console.log(resposta);
            setListaAjudasItens(resposta);

            nProgress.done();
        }

        const query = router?.query?.query ?? '';
        setQueryBuscada(query.toString());
        handleBuscar(query.toString());

        paginaCarregada(true, 300, 600, setIsLoaded);
    }, [router.query]);

    if (!isLoaded) {
        return false;
    }

    return (
        <section className={`${Styles.main} paddingPadrao paddingPadraoMargemGrande`}>
            {/* #1 - Título */}
            <div className={Styles.divTitulo}>
                <span>Central de ajuda</span>

                <div className='animate__animated animate__pulse animate__slower animate__infinite'>
                    <Image src={EmojiMedicacao} alt='' width={50} height={63} />
                </div>
            </div>

            {/* #2 - Input para filtragem dos tópicos */}
            <AjudaInputPesquisaTopico topicoBuscado={queryBuscada} />

            {/* #3 - "Header" do restauldo da busca */}
            <div className='margem3 centralizarTexto'>
                <span className='titulo'>Resultado da busca</span>
                <br />
                <span className={Styles.textoPequeno}> {listaAjudasItens?.length ?? 0} {listaAjudasItens?.length === 1 ? 'resultado' : 'resultados'}</span>
            </div>
            <span className='margem2'></span>

            {/* #4 - Resultados da busca */}
            <AjudaListaItens
                listaAjudasItens={listaAjudasItens}
                queryBuscada={queryBuscada}
                isMargemTop={false}
                isExibirTopico={true}
            />

            {/* Espaço a mais */}
            <div className='espacoBottom'></div>
        </section>
    )
}


