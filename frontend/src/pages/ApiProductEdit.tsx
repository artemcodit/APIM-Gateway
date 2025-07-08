import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { getApiProduct, updateApiProduct, getApisForProduct, addApiToProduct, removeApiFromProduct } from '../services/apiProduct';
import { getApis } from '../services/api';
import type { ApiProduct, ApiProductCreate } from '../services/apiProduct';
import type { Api } from '../services/api';

const ApiProductEditPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [product, setProduct] = useState<ApiProduct | null>(null);
  const [productApis, setProductApis] = useState<Api[]>([]);
  const [allApis, setAllApis] = useState<Api[]>([]);
  const [selectedApi, setSelectedApi] = useState('');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchData = async () => {
      if (!id) return;
      try {
        setLoading(true);
        const [productRes, productApisRes, allApisRes] = await Promise.all([
          getApiProduct(id),
          getApisForProduct(id),
          getApis(),
        ]);
        setProduct(productRes.data);
        setProductApis(productApisRes.data);
        setAllApis(allApisRes.data);
      } catch (err) {
        setError('Failed to fetch data');
        console.error(err);
      } finally {
        setLoading(false);
      }
    };
    fetchData();
  }, [id]);

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    if (product) {
      const { name, value } = e.target;
      setProduct({ ...product, [name]: value });
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!id || !product) return;
    try {
      const updateData: Partial<ApiProductCreate> = { name: product.name, description: product.description };
      await updateApiProduct(id, updateData);
      navigate('/api-products');
    } catch (err) {
      setError('Failed to update product');
      console.error(err);
    }
  };

  const handleAddApi = async () => {
    if (!id || !selectedApi) return;
    try {
      await addApiToProduct(id, selectedApi);
      const apiToAdd = allApis.find(a => a.id === selectedApi);
      if (apiToAdd) {
        setProductApis([...productApis, apiToAdd]);
      }
      setSelectedApi('');
    } catch (err) {
      setError('Failed to add API to product');
      console.error(err);
    }
  };

  const handleRemoveApi = async (apiId: string) => {
    if (!id) return;
    try {
      await removeApiFromProduct(id, apiId);
      setProductApis(productApis.filter(a => a.id !== apiId));
    } catch (err) {
      setError('Failed to remove API from product');
      console.error(err);
    }
  };

  if (loading) return <p>Loading...</p>;
  if (error) return <p className="text-red-500">{error}</p>;
  if (!product) return <p>Product not found.</p>;

  const availableApis = allApis.filter(api => !productApis.some(pa => pa.id === api.id));

  return (
    <div>
      <h1 className="text-2xl font-bold mb-4">Edit API Product</h1>
      <form onSubmit={handleSubmit} className="mb-8">
        <div className="mb-4">
          <label className="block text-gray-700 dark:text-gray-300">Name</label>
          <input
            type="text"
            name="name"
            value={product.name}
            onChange={handleInputChange}
            className="w-full px-3 py-2 border rounded dark:bg-gray-700 dark:border-gray-600"
            required
          />
        </div>
        <div className="mb-4">
          <label className="block text-gray-700 dark:text-gray-300">Description</label>
          <textarea
            name="description"
            value={product.description || ''}
            onChange={handleInputChange}
            className="w-full px-3 py-2 border rounded dark:bg-gray-700 dark:border-gray-600"
          />
        </div>
        <button type="submit" className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">
          Save Changes
        </button>
      </form>

      <div className="mt-8">
        <h2 className="text-xl font-bold mb-4">Manage APIs in this Product</h2>
        
        {/* Add API to Product */}
        <div className="flex items-center mb-4">
          <select 
            value={selectedApi}
            onChange={(e) => setSelectedApi(e.target.value)}
            className="w-full px-3 py-2 border rounded dark:bg-gray-700 dark:border-gray-600 mr-2"
          >
            <option value="">Select an API to add</option>
            {availableApis.map(api => (
              <option key={api.id} value={api.id}>{api.name}</option>
            ))}
          </select>
          <button onClick={handleAddApi} className="bg-green-500 hover:bg-green-700 text-white font-bold py-2 px-4 rounded">
            Add API
          </button>
        </div>

        {/* List of APIs in Product */}
        <div className="bg-white dark:bg-gray-800 shadow-md rounded-lg">
          <table className="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
            <thead className="bg-gray-50 dark:bg-gray-700">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">API Name</th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Actions</th>
              </tr>
            </thead>
            <tbody className="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
              {productApis.map((api) => (
                <tr key={api.id}>
                  <td className="px-6 py-4 whitespace-nowrap">{api.name}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                    <button onClick={() => handleRemoveApi(api.id)} className="text-red-600 hover:text-red-900">Remove</button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
};

export default ApiProductEditPage;
