const ACCESS_TOKEN_KEY = 'accessToken';
const REFRESH_TOKEN_KEY = 'refreshToken';
const USER_ID_KEY = 'userId';
const USER_ROLE_KEY = 'userRole';

export const getAccessToken = (): string | null => localStorage.getItem(ACCESS_TOKEN_KEY);

export const getUserId = (): string | null => localStorage.getItem(USER_ID_KEY);

export const getUserRole = (): string | null => localStorage.getItem(USER_ROLE_KEY);

export const isAuthenticated = (): boolean => Boolean(getAccessToken());

export const isAdmin = (): boolean => getUserRole() === 'Admin';

export const setSession = (accessToken: string, refreshToken: string, userId: string | null, role: string | null): void => {
  localStorage.setItem(ACCESS_TOKEN_KEY, accessToken);
  localStorage.setItem(REFRESH_TOKEN_KEY, refreshToken);
  if (userId) {
    localStorage.setItem(USER_ID_KEY, userId);
  }
  if (role) {
    localStorage.setItem(USER_ROLE_KEY, role);
  } else {
    localStorage.removeItem(USER_ROLE_KEY);
  }
};

export const clearSession = (): void => {
  localStorage.removeItem(ACCESS_TOKEN_KEY);
  localStorage.removeItem(REFRESH_TOKEN_KEY);
  localStorage.removeItem(USER_ID_KEY);
  localStorage.removeItem(USER_ROLE_KEY);
};

export const authStorageKeys = {
  accessToken: ACCESS_TOKEN_KEY,
  refreshToken: REFRESH_TOKEN_KEY,
  userId: USER_ID_KEY,
  userRole: USER_ROLE_KEY,
} as const;
