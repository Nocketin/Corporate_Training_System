import { LoginPage } from './pages/Login/LoginPage';
import { RegisterPage } from './pages/Register/RegisterPage';

export const App = () => {
  const path = window.location.pathname;
  
  if (path === '/register') {
    return <RegisterPage />;
  }
  
  return <LoginPage />;
};

