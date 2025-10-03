import { CSSProperties, ChangeEvent, Dispatch, ReactNode, SetStateAction, useEffect, useRef, useState } from 'react';
import Swal from 'sweetalert2';
import styles from './index.module.scss';

interface iProps<T> {
    fieldName: keyof T;
    formData: T;
    setFormData?: Dispatch<SetStateAction<T>>;

    title?: string;
    classes?: string;
    style?: CSSProperties;
    placeholder?: string;
    isDisabled?: boolean;
    isObligatory?: boolean;
    showPreview?: boolean;
    svg_component?: ReactNode;
}

export default function InputImage<T>({
    fieldName,
    formData,
    setFormData,

    title = '',
    classes = '',
    style = {},
    placeholder,
    isDisabled = false,
    isObligatory = false,
    showPreview = true,
    svg_component = null,
}: iProps<T>) {

    const value_formData = formData?.[fieldName] as unknown as number[] | null;
    const [preview, setPreview] = useState<string | null>(null);
    const [fileName, setFileName] = useState<string | null>(null);

    const fileInputRef = useRef<HTMLInputElement>(null);

    const defaultHandleChange = (e: ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];

        if (!file) {
            return;
        }

        setFileName(file.name);

        // salva o próprio File no formData
        setFormData && setFormData((prev: T) => ({
            ...prev,
            [fieldName]: file as T[keyof T],
        }));

        // gera preview
        const objectUrl = URL.createObjectURL(file);
        setPreview(objectUrl);
    };

    const removeImage = () => {
        setPreview(null);
        setFileName(null);

        setFormData && setFormData((prev: T) => ({
            ...prev,
            [fieldName]: [] as unknown as T[keyof T],
        }));

        // Resetar o input pra permitir selecionar mesma imagem de novo
        if (fileInputRef.current) {
            fileInputRef.current.value = '';
        }
    };

    useEffect(() => {
        if (!value_formData || value_formData.length === 0) {
            setPreview(null);
            setFileName(null);
            return;
        }
    }, [value_formData]);

    return (
        <div className={styles.main}>
            {
                title && (
                    <span className={styles.title}>
                        {title} {isObligatory && <span className={styles.obligatory}>*</span>}
                    </span>
                )
            }

            <div className={styles.wrapper}>
                {svg_component && svg_component}

                <label className={`${styles.uploadButton} ${isDisabled ? styles.disabled : ''}`}>
                    {fileName ? 'Selecionar outra imagem' : placeholder ?? 'Selecionar imagem'}

                    <input
                        ref={fileInputRef}
                        type='file'
                        accept='image/*'
                        className={classes}
                        style={style}
                        disabled={isDisabled}
                        onChange={defaultHandleChange}
                        hidden={true}
                    />
                </label>

                {
                    showPreview && fileName && (
                        <button
                            type='button'
                            className={styles.simpleButton}
                            onClick={() => {
                                if (preview) {
                                    Swal.fire({
                                        imageUrl: preview,
                                        imageAlt: 'Preview',
                                        titleText: fileName,
                                        showConfirmButton: false
                                    });
                                }
                            }}
                        >
                            Visualizar {fileName}
                        </button>
                    )
                }

                {
                    fileName && !isDisabled && (
                        <button
                            type='button'
                            className={styles.simpleButton}
                            onClick={removeImage}
                        >
                            Remover
                        </button>
                    )
                }
            </div>
        </div>
    )
}