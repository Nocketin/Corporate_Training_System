import { Outlet } from 'react-router-dom';
import { AppSidebar } from './AppSidebar';
import styles from './AppLayout.module.scss';

export const AppLayout = () => (
  <div className={styles.shell}>
    <AppSidebar />
    <main className={styles.main}>
      <Outlet />
    </main>
  </div>
);
