import { createContext, useContext, useReducer } from "react";

const ContactsContext = createContext();

const initialState = {
  contacts: [],
  isLoading: false,
  error: null,
  selectedContact: null,
  search: "",
  sortBy: "name",
  sortDirection: "asc",
  isEditing: false,
  isAdding: false,
  isEditModalOpen: false,
  isCreateModalOpen: false,
};

function contactsReducer(state, action) {
  switch (action.type) {
    case "FETCH_CONTACTS_START":
      return { ...state, isLoading: true, error: null };
    case "FETCH_CONTACTS_SUCCESS":
      return { ...state, isLoading: false, contacts: action.payload };
    case "FETCH_CONTACTS_ERROR":
      return { ...state, isLoading: false, error: action.payload };
    case "ADD_CONTACT":
      return {
        ...state,
        contacts: [...state.contacts, action.payload],
      };
    case "DELETE_CONTACT":
      return {
        ...state,
        contacts: state.contacts.filter(
          (contact) => contact.id !== action.payload
        ),
      };
    case "UPDATE_CONTACT":
      return {
        ...state,
        contacts: state.contacts.map((contact) =>
          contact.id === action.payload.id? action.payload : contact
        ),
      };
    case "SELECT_CONTACT":
      return { ...state, selectedContact: action.payload };
    case "OPEN_EDIT_MODAL":
      return { ...state, isEditModalOpen: true };
    case "CLOSE_EDIT_MODAL":
      return { ...state, isEditModalOpen: false, selectedContact: null };
    case "OPEN_CREATE_MODAL":
      return { ...state, isCreateModalOpen: true };
    case "CLOSE_CREATE_MODAL":
      return { ...state, isCreateModalOpen: false };
    default:
      return state;
  }
}

export const ContactsProvider = ({ children }) => {
  const [state, dispatch] = useReducer(contactsReducer, initialState);

  return (
    <ContactsContext.Provider value={{ state, dispatch }}>
      {children}
    </ContactsContext.Provider>
  );
};

export const useContactsContext = () => {
  return useContext(ContactsContext);
};
