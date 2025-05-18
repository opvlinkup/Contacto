import ContactsPage from "./pages/ContactsPage";
import { ContactsProvider } from "../src/context/ContactsContext";
import { ThemeProvider, CssBaseline } from "@mui/material";
import theme from "../src/themes/darkTheme";

function App() {
  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <ContactsProvider>
        <ContactsPage />
      </ContactsProvider>
    </ThemeProvider>
  );
}

export default App;
