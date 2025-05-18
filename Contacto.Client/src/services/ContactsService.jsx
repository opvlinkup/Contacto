import api from "./api";

const contactsService = {
  getById: async (id) => {
    try {
      const res = await api.get(`/contacts/${id}`);
      return res.data;
    } catch (error) {
      handleError(error);
    }
  },

  getAll: async () => {
    try {
      const res = await api.get("/contacts");
      return res.data;
    } catch (error) {
      handleError(error);
    }
  },
  getAllPaged: async (params = {}) => {
    try {
      const res = await api.get("/contacts/p", { params });
      return res.data;
    } catch (error) {
      handleError(error);
    }
  },
  create: async (contactData) => {
    try {
      const res = await api.post("/contacts", contactData);
      return res.data;
    } catch (error) {
      handleError(error);
    }
  },

  update: async (id, contactData) => {
    try {
      const res = await api.patch(`/contacts/${id}`, contactData);
      return res.data;
    } catch (error) {
      handleError(error);
    }
  },

  remove: async (id) => {
    try {
      await api.delete(`/contacts/${id}`);
      return id;
    } catch (error) {
      handleError(error);
    }
  },
};

const handleError = (error) => {
  console.error(
    "Contacts Service Error:",
    error?.response || error?.message || error
  );
  throw error;
};

export default contactsService;