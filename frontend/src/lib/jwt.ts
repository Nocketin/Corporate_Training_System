function parsePayload(token: string): Record<string, unknown> | null {
  try {
    const parts = token.split('.');
    if (parts.length < 2) return null;
    const payload = parts[1].replace(/-/g, '+').replace(/_/g, '/');
    return JSON.parse(atob(payload)) as Record<string, unknown>;
  } catch {
    return null;
  }
}

export function parseJwtSub(token: string): string | null {
  const json = parsePayload(token);
  return typeof json?.sub === 'string' ? json.sub : null;
}

export function parseJwtRole(token: string): string | null {
  const json = parsePayload(token);
  if (!json) return null;
  const role =
    json.role ??
    json['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
  return typeof role === 'string' ? role : null;
}
