import { Outlet } from 'react-router-dom';
import styles from './AuthLayout.module.scss';

export const AuthLayout = () => (
  <div className={styles.page}>
    <aside className={styles.promo}>
      <div className={styles.promoInner}>
        <p className={styles.promoLabel}>CTS Academy</p>
        <h1 className={styles.promoTitle}>Корпоративное обучение команды</h1>
        <p className={styles.promoText}>
          Курсы по .NET, микросервисам и soft skills. Структурированные программы от основ до
          продвинутых модулей.
        </p>
        <ul className={styles.promoList}>
          <li>Микросервисы и Kafka</li>
          <li>React и TypeScript</li>
          <li>DevOps и лидерство</li>
        </ul>
      </div>
    </aside>
    <div className={styles.formArea}>
      <Outlet />
    </div>
  </div>
);
