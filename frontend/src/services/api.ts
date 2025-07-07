import axios from 'axios';

const api = axios.create({
  baseURL: '/api',
});

api.interceptors.request.use((config) => {
  const apiKey = 'SuperSecretApiKey';
  if (apiKey) {
    config.headers['X-Api-Key'] = apiKey;
  }
  return config;
});

export default api;
