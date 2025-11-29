import InputMask from '@/app/components/input/text';
import Lupa from '@/app/components/svg/lupa/lupa';
import ROUTES from '@/app/consts/routes';
import toast from '@/app/functions/toast';
import { useRouter } from 'next/navigation';
import { KeyboardEvent, useState } from 'react';
import styles from './index.module.scss';

interface iProps {
    keySearch: string | undefined;
    hasAltStyle?: boolean;
}

interface iFormData {
    key: string | undefined;
}

export default function AjudaSearchInput({ keySearch, hasAltStyle = false }: iProps) {

    const router = useRouter();

    function handleKeyDown(e: KeyboardEvent<HTMLInputElement>) {
        if (e.key === 'Enter') {
            handleSearch();
        }
    }

    const [formData, setFormData] = useState<iFormData>({
        key: keySearch
    });

    function handleSearch() {
        if (!formData.key) {
            toast({ content: 'O filtro não pode estar nulo.' });
            return;
        }

        router.push(`${ROUTES.ETC_AJUDA}/topico?i=${formData.key}`);
    }

    return (
        <div className={styles.search}>
            <div className={`${styles.input} ${hasAltStyle && styles.altStyle}`}>
                <InputMask
                    title='Busque aqui :)'
                    fieldName='key'
                    type='text'
                    formData={formData}
                    setFormData={setFormData}
                    placeholder='Procure por um tópico como "agendamento" ou "empresa", por exemplo'
                    handleKeyDown={handleKeyDown}
                />

                <div
                    className={`${styles.icon} contrastOnHover`}
                    title='Buscar tópico'
                    onClick={() => handleSearch()}
                >
                    <Lupa />
                </div>
            </div>
        </div>
    )
}