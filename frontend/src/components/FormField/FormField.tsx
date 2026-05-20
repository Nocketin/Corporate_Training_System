import type { InputHTMLAttributes } from 'react';
import styles from './FormField.module.scss';

export type FormFieldProps = {
  label: string;
} & InputHTMLAttributes<HTMLInputElement>;

export const FormField = ({ label, className, id, ...inputProps }: FormFieldProps) => (
  <label className={styles.wrap} htmlFor={id}>
    <span className={styles.label}>{label}</span>
    <input id={id} className={`${styles.input} ${className ?? ''}`.trim()} {...inputProps} />
  </label>
);
