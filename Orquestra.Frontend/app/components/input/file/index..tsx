import { ChangeEvent, RefObject } from 'react';
import styles from './index.module.scss';

type TypeMIMEs =
    | 'audio/*'
    | 'video/*'
    | 'image/*'
    | 'application/pdf'
    | 'application/msword'
    | 'application/vnd.openxmlformats-officedocument.wordprocessingml.document'
    | 'application/vnd.ms-excel'
    | 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
    | 'application/vnd.ms-powerpoint'
    | 'application/vnd.openxmlformats-officedocument.presentationml.presentation'
    | 'application/zip'
    | 'application/octet-stream'
    | 'text/plain'
    | 'text/csv';

interface iProps {
    refInput: RefObject<HTMLInputElement>;
    handleFile: (file: File | null) => Promise<void>;
    accept?: TypeMIMEs[];
}

export default function InputFile({ refInput, handleFile, accept }: iProps) {

    async function handleFileInputChange(e: ChangeEvent<HTMLInputElement>) {
        const selectedFile = e.target.files && e.target.files[0];

        if (selectedFile && handleFile) {
            handleFile(selectedFile);

            if (refInput.current) {
                // console.log('refInput clean');
                refInput.current.value = '';
            }
        }
    }

    return (
        <input
            type='file'
            className={styles.input}
            ref={refInput}
            style={{ display: 'none' }}
            onChange={handleFileInputChange}
            accept={accept && accept.join(',')}
        />
    )
}