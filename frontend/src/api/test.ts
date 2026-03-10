import axios from 'axios';

const BASE_URL = 'http://127.0.0.1:5000';
const TEST_USER = {
    email: 'test@gamil.com',
    password: '123',
};

async function main() {
    let accessToken: string | null = null;
    
    try {
        console.log('Registering user...');
        const registerResponse = await axios.post(`${BASE_URL}/api/auth/register`, TEST_USER);
        console.log('Register response:', registerResponse.data);
    } catch (err: any) {
        
        if (err.response?.status === 500) {
            console.log('User probably already exists. Continuing to login...');
        } else {
            console.error('Register error:', err.message, err.response?.data);
            return;
        }
    }
    
    try {
        console.log('Logging in...');
        const loginResponse = await axios.post(`${BASE_URL}/api/auth/login`, TEST_USER);
        console.log('Login response:', loginResponse.data);
        accessToken = loginResponse.data.accessToken;
    } catch (err: any) {
        console.error('Login error:', err.message, err.response?.data);
        return;
    }
    
    if (!accessToken) {
        console.error('No access token, cannot test protected endpoint.');
        return;
    }

    try {
        console.log('Accessing protected endpoint...');
        const protectedResponse = await axios.get(`${BASE_URL}/api/secret`, {
            headers: {
                Authorization: `Bearer ${accessToken}`,
            },
        });
        console.log('Protected data:', protectedResponse.data);
    } catch (err: any) {
        console.error('Protected endpoint error:', err.message, err.response?.data);
    }
}

main();