import React, { Component } from "react";
import { connect } from "react-redux";
import ProductFilter from "../components/productFilter";
import Header from "../components/header";
import { fetchAllStoreProducts } from "../actions/storeActions";
import StoreProductView from "../components/storeProductView";
import StoreSideBar from "../components/storeSideBar";
import Dialog from "@material-ui/core/Dialog";
import DialogActions from "@material-ui/core/DialogActions";
import DialogContent from "@material-ui/core/DialogContent";
import DialogContentText from "@material-ui/core/DialogContentText";
import DialogTitle from "@material-ui/core/DialogTitle";

class StoreProductsPage extends Component {
  state = {
    isDialogOpen: false,
    name: "",
    category: "",
    amount: "",
    price: "",
  };

  handleChange = (event) => {
    this.setState({
      [event.target.name]: event.target.value,
    });
  };
  componentDidMount(prevProps) {
    console.log(this.props.match.params.guid);
    if (this.props !== prevProps) {
      this.props.dispatch(fetchAllStoreProducts(this.props.match.params.guid));
    }
  }
  handleOpen = () => {
    this.setState({ isDialogOpen: true });
  };
  handleClose = () => {
    this.setState({ isDialogOpen: false });
  };
  handleSubmit = () => {
    if (
      !this.state.name ||
      !this.state.category ||
      !this.state.amount ||
      !this.state.price
    ) {
      return;
    } else {
      this.setState({ isDialogOpen: false });
    }
  };
  render() {
    return (
      <React.Fragment>
        <Header />
        <section className="section-content padding-y">
          <div className="container" style={{ maxWidth: "1500px" }}>
            <div className="container" style={{ maxWidth: "1400px" }}>
              <section className="text-center border-bottom">
                <h1 className="jumbotron-heading">{`${this.props.storeInfo.storeName} Products`}</h1>
                <p>
                  <button
                    onClick={this.handleOpen}
                    className="btn btn-outline-primary my-2"
                  >
                    Add Product
                  </button>
                  <Dialog
                    open={this.state.isDialogOpen}
                    onClose={this.handleClose}
                    aria-labelledby="form-dialog-title"
                  >
                    <DialogTitle id="form-dialog-title">
                      Add product
                    </DialogTitle>
                    <form>
                      <DialogContent>
                        <DialogContentText>
                          Please fill out the following product details:
                        </DialogContentText>
                        <label>Name:</label>
                        <input
                          type="text"
                          class="form-control mb-2"
                          name="name"
                          required
                          value={this.state.name}
                          onChange={this.handleChange}
                        ></input>
                        <label>category:</label>
                        <input
                          type="text"
                          class="form-control mb-2"
                          name="category"
                          required
                          value={this.state.category}
                          onChange={this.handleChange}
                        ></input>
                        <label>amount:</label>
                        <input
                          type="text"
                          class="form-control mb-2"
                          name="amount"
                          required
                          value={this.state.amount}
                          onChange={this.handleChange}
                        ></input>
                        <label>price:</label>
                        <input
                          type="text"
                          class="form-control mb-2"
                          name="price"
                          required
                          value={this.state.price}
                          onChange={this.handleChange}
                        ></input>
                      </DialogContent>
                      <DialogActions>
                        <button
                          className="btn btn-outline-primary my-2"
                          onClick={this.handleClose}
                        >
                          Cancel
                        </button>
                        <button
                          className="btn btn-outline-primary my-2"
                          onClick={this.handleSubmit}
                        >
                          Add
                        </button>
                      </DialogActions>
                    </form>
                  </Dialog>
                </p>
              </section>
            </div>
            <br />
            <div className="row">
              <StoreSideBar
                isProducts="true"
                guid={this.props.guid}
                colClass="col-2"
              />
              <ProductFilter
                maxWidthStyle="290px"
                categories={[
                  ...new Set(
                    this.props.filteredProducts?.map(
                      (product) => product.category
                    )
                  ),
                ]}
                products={this.props.products}
                filteredProducts={this.props.filteredProducts}
              />
              <main className="col-sm-6">
                <StoreProductView guid={this.props.match.params.guid} />
              </main>
            </div>
          </div>
        </section>
      </React.Fragment>
    );
  }
}

const mapStateToProps = (store) => {
  return {
    products: store.store.products,
    filteredProducts: store.store.filteredProducts,
    storeInfo: store.store.storeInfo,
  };
};

export default connect(mapStateToProps)(StoreProductsPage);
