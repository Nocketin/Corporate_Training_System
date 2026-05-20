import { api } from './axiosInstance';

export interface UserListItem {
  id: string;
  email: string;
  username: string;
  role: string;
  createdAt: string;
}

export async function fetchUsers(): Promise<UserListItem[]> {
  const { data } = await api.get<UserListItem[]>('/api/auth/users');
  return data;
}

export async function updateUserRole(userId: string, role: string): Promise<UserListItem> {
  const { data } = await api.patch<UserListItem>(`/api/auth/users/${userId}/role`, { role });
  return data;
}
