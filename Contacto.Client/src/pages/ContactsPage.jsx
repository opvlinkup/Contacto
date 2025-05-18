import { useEffect, useState } from "react";
import {
  Container,
  Box,
  Typography,
  Button,
  Toolbar,
  AppBar,
  Snackbar,
  Alert,
  Fab,
} from "@mui/material";
import ContactList from "../components/ContactList";
import ContactForm from "../components/ContactForm";
import contactsService from "../services/ContactsService";
import { useContactsContext } from "../context/ContactsContext";
import AddIcon from "@mui/icons-material/Add";

const ContactsPage = () => {
  const { state, dispatch } = useContactsContext();
  const { isEditModalOpen, isCreateModalOpen, selectedContact } = state;
  const [snackbarOpen, setSnackbarOpen] = useState(false);
  const [snackbarMessage, setSnackbarMessage] = useState("");
  const [snackbarSeverity, setSnackbarSeverity] = useState("success");

  useEffect(() => {
    const fetchContacts = async () => {
      dispatch({ type: "FETCH_CONTACTS_START" });
      try {
        const contacts = await contactsService.getAllPaged();
        dispatch({ type: "FETCH_CONTACTS_SUCCESS", payload: contacts });
      } catch (error) {
        dispatch({ type: "FETCH_CONTACTS_ERROR", payload: error });
      }
    };

    fetchContacts();
  }, [dispatch]);

  const handleCreateContact = async (values) => {
    try {
      const newContact = await contactsService.create(values);
      dispatch({ type: "ADD_CONTACT", payload: newContact });
      dispatch({ type: "CLOSE_CREATE_MODAL" });
      showSnackbar("Contact created successfully!");
    } catch (error) {
      showSnackbar("Failed to create contact.", "error");
      throw error;
    }
  };

  const showSnackbar = (message, severity = "success") => {
    setSnackbarMessage(message);
    setSnackbarSeverity(severity);
    setSnackbarOpen(true);
  };

  const handleSnackbarClose = () => {
    setSnackbarOpen(false);
  };

  const handleUpdateContact = async (values) => {
    try {
      console.log("Updating contact:", values);
      console.log("selectedContact:", selectedContact);

      const updatedContact = await contactsService.update(
        selectedContact.id,
        values
      );
      dispatch({ type: "CLOSE_EDIT_MODAL" });
      dispatch({ type: "UPDATE_CONTACT", payload: updatedContact });
      showSnackbar("Contact updated successfully!");
    } catch (error) {
      showSnackbar("Failed to update contact.", "error");
      dispatch({ type: "CLOSE_EDIT_MODAL" });
      throw error;
    }
  };

  return (
    <>
      <AppBar position="static">
        <Toolbar>
          <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
            CONTACTO
          </Typography>
        </Toolbar>
      </AppBar>
      <Container maxWidth="lg" sx={{ mt: 4 }}>
        <ContactList />
      </Container>

      <Fab
        color="warning"
        aria-label="add"
        onClick={() => dispatch({ type: "OPEN_CREATE_MODAL" })}
        sx={{
          position: "fixed",
          bottom: 24,
          right: 24,
          zIndex: 1000,
        }}
      >
        <AddIcon />
      </Fab>

      <ContactForm
        showSnackbar={showSnackbar}
        isOpen={isCreateModalOpen}
        onClose={() => dispatch({ type: "CLOSE_CREATE_MODAL" })}
        onSubmit={handleCreateContact}
        isEdit={false}
      />

      <ContactForm
        showSnackbar={showSnackbar}
        isOpen={isEditModalOpen}
        onClose={() => dispatch({ type: "CLOSE_EDIT_MODAL" })}
        onSubmit={handleUpdateContact}
        initialValues={selectedContact}
        isEdit={true}
      />
<Snackbar
  open={snackbarOpen}
  autoHideDuration={3000}
  onClose={handleSnackbarClose}
  anchorOrigin={{ vertical: "bottom", horizontal: "left" }}
>
  <Alert
    onClose={handleSnackbarClose}
    severity={snackbarSeverity}
    sx={{
      width: "100%",
      backgroundColor:
        snackbarSeverity === "error"
          ? "#f7096d"
          : snackbarSeverity === "success"
          ? "#9cfa50"
          : snackbarSeverity === "warning"
          ? "#ffc107"
          : snackbarSeverity === "info"
          ? "#2196f3"
          : undefined,
      color: "#000",
    }}
  >
    {snackbarMessage}
  </Alert>
</Snackbar>

    </>
  );
};

export default ContactsPage;
