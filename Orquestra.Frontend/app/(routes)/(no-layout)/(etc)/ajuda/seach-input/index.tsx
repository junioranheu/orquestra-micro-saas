import styles from '@/app/(routes)/(no-layout)/(etc)/ajuda/page.module.scss';
import InputMask from '@/app/components/input/text';
import Lupa from '@/app/components/svg/lupa/lupa';
import ROUTES from '@/app/consts/routes';
import handleGetPropName from '@/app/functions/get.propName';
import { handleInputFormStateChange } from '@/app/functions/set.formState';
import { useRouter } from 'next/navigation';
import { KeyboardEvent, useEffect, useState } from 'react';

interface iProps {
    key: string | undefined;
}

interface iFormData {
    key: string | undefined;
}

export default function AjudaSearchInput({ key }: iProps) {

    useEffect(() => {
        console.log(key);
    }, [key]);

    const router = useRouter();

    function handleKeyDown(e: KeyboardEvent<HTMLInputElement>) {
        if (e.key === 'Enter') {
            handleSearch();
        }
    }

    const [formData, setFormData] = useState<iFormData>({
        key: key
    });

    function handleSearch() {
        if (!formData.key) {
            return;
        }

        router.push(`${ROUTES.ETC_AJUDA}/busca?q=${formData.key}`);
    }

    return (
        <div className={styles.search}>
            <InputMask
                title='Busque aqui :)'
                objectFormData={handleGetPropName(formData, x => x.key ?? '')}
                placeholder='Procure por um tópico como "agendamento" ou "empresa", por exemplo'
                type='text'
                handleChange={(e) => handleInputFormStateChange(e, setFormData)}
                handleKeyDown={(e) => handleKeyDown(e)}
            />

            <div
                className={`${styles.icon} contrastOnHover`}
                title='Buscar tópico'
                onClick={() => handleSearch()}
            >
                <Lupa />
            </div>
        </div>
    )
}

