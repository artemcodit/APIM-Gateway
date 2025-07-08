import axios from 'axios';
import type { ApiProduct } from './apiProduct';

export interface Api {
  id: string;
  name: string;
  route: string;
  upstreamUrl: string;
  methods: string[];
  hosts: string[];
  tags: string[];
  isEnabled: boolean;
  apiProducts?: ApiProduct[];
}

export interface ApiCreateData {
    name: string;
    route: string;
    upstreamUrl: string;
    methods: string[];
    hosts: string[];
    apiProductId: string | null;
}

export interface ApiUpdateData {
    name: string;
    route: string;
    upstreamUrl: string;
    methods: string[];
    hosts: string[];
    tags: string[];
    isEnabled: boolean;
    apiProductId: string | null;
}

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

export const getApis = () => api.get<Api[]>('/apis');
export const getApiById = (id: string) => api.get<Api>(`/apis/${id}`);
export const createApi = (data: ApiCreateData) => api.post<Api>('/apis', data);
export const updateApi = (id: string, data: ApiUpdateData) => api.put(`/apis/${id}`, data);

export default api;
