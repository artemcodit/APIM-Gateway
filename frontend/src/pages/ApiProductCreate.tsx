import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { createApiProduct } from '../services/apiProduct';
import type { ApiProductCreate } from '../services/apiProduct';

const ApiProductCreatePage: React.FC = () => {
  const [product, setProduct] = useState<ApiProductCreate>({ name: '', description: '' });
  const [errors, setErrors] = useState<{ [key: string]: string }>({});
  const navigate = useNavigate();

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    setProduct({ ...product, [name]: value });
  };

  const validate = () => {
    const newErrors: { [key: string]: string } = {};
    if (!product.name.trim()) {
      newErrors.name = 'Name is required';
    }
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validate()) {
      return;
    }

    try {
      await createApiProduct(product);
      navigate('/api-products');
    } catch (err) {
      setErrors({ form: 'Failed to create API product' });
      console.error(err);
    }
  };

  return (
    <div>
      <h1 className="text-2xl font-bold mb-4">Create API Product</h1>
      {errors.form && <p className="text-red-500 mb-4">{errors.form}</p>}
      <form onSubmit={handleSubmit}>
        <div className="mb-4">
          <label className="block text-gray-700 dark:text-gray-300">Name</label>
          <input
            type="text"
            name="name"
            value={product.name}
            onChange={handleInputChange}
            className="w-full px-3 py-2 border rounded dark:bg-gray-700 dark:border-gray-600"
          />
          {errors.name && <p className="text-red-500 text-xs mt-1">{errors.name}</p>}
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
          Create
        </button>
      </form>
    </div>
  );
};

export default ApiProductCreatePage;
