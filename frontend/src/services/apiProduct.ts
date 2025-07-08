import api from './api';

export interface ApiProduct {
  id: string;
  name: string;
  description?: string;
  apiCount: number;
}

export interface ApiProductCreate {
  name: string;
  description?: string;
}

export const getApiProducts = () => api.get<ApiProduct[]>('/api-products');

export const getApiProduct = (id: string) => api.get<ApiProduct>(`/api-products/${id}`);

export const createApiProduct = (data: ApiProductCreate) => api.post<ApiProduct>('/api-products', data);

export const updateApiProduct = (id: string, data: Partial<ApiProductCreate>) => api.patch(`/api-products/${id}`, data);

export const deleteApiProduct = (id: string) => api.delete(`/api-products/${id}`);

export const getApisForProduct = (id: string) => api.get(`/api-products/${id}/apis`);

export const addApiToProduct = (productId: string, apiId: string) => api.post(`/api-products/${productId}/apis`, { apiId });

export const removeApiFromProduct = (productId: string, apiId: string) => api.delete(`/api-products/${productId}/apis/${apiId}`);
