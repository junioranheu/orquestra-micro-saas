'use client';
import { iAjudaItem } from '@/app/(routes)/(no-layout)/etc/ajuda/page';
import useTitle from '@/app/hooks/useTitle';
import styles from './page.module.scss';

interface iProps {
    ajudaItem: iAjudaItem;
}

export default function ItemAjuda({ ajudaItem }: iProps) {

    useTitle('Central de ajuda');

    return (
        <section className={styles.main}>
            {/* <div className={Styles.divVoltar} onClick={() => Router.push(`/ajuda/topico/${ajudaItem?.ajudaTopicoId}/${ajustarUrl(removerHTML(ajudaItem?.ajudasTopicos?.topico))}`)}>
                <SetaDois width={16} url={null} title='Voltar' isCorPrincipal={true} />
                <span className='texto pointer cor-principal-hover' title={removerHTML(ajudaItem?.ajudasTopicos?.topico)}>Voltar ao tópico original</span>
            </div>

            <div className={`${Styles.divTitulo} margem3`}>
                <span className={Styles.textoPequeno}>Central de ajuda / {removerHTML(ajudaItem?.ajudasTopicos?.topico)}</span>
                <span className={Styles.titulo}>{ajudaItem?.titulo}</span>
            </div>

            <div className='margem3'>
                <div className={Styles.conteudoHTML} dangerouslySetInnerHTML={{ __html: (ajudaItem?.conteudoHtml ?? '') }} />
            </div> */}
        </section>
    )
}