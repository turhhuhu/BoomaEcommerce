import React, { Component } from "react";
import { connect } from "react-redux";
import Dialog from "@material-ui/core/Dialog";
import DialogActions from "@material-ui/core/DialogActions";
import DialogContent from "@material-ui/core/DialogContent";
import DialogContentText from "@material-ui/core/DialogContentText";
import DialogTitle from "@material-ui/core/DialogTitle";
import { Alert } from "@material-ui/lab";
import Select from "react-select";
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";
import {
  addStoreRootDiscount,
  addStoreSubDiscount,
  fetchStoreDiscounts,
} from "../actions/storeActions";

const typeOptions = [
  { value: "composite", label: "Composite", name: "type" },
  { value: "basket", label: "Basket", name: "type" },
  {
    value: "category",
    label: "Category",
    name: "type",
  },
  {
    value: "product",
    label: "Product",
    name: "type",
  },
];
const operatorOptions = [
  { value: "max", label: "Max", name: "operator" },
  { value: "sum", label: "Sum", name: "operator" },
];

class AddStorePolicyDiscount extends Component {
  state = {
    error: undefined,
    type: "",
    productGuid: "",
    category: "",
    startTime: "",
    endTime: "",
    percentage: 0,
    isMenuOpen: false,
  };

  handleChange = (event) => {
    if (event?.target) {
      this.setState({
        [event.target.name]: event.target.value,
      });
    } else {
      this.setState({
        [event.name]: event.value,
      });
    }
  };

  handleClose = (event) => {
    event.preventDefault();
    this.props.closeDialog();
  };

  handleOpenMenu = () => {
    this.setState({ isMenuOpen: true });
  };

  handleCloseMenu = () => {
    this.setState({ isMenuOpen: false });
  };

  dynamicStyle = () => {
    if (this.state.isMenuOpen) {
      if (this.props.isRoot) {
        return { height: "350px" };
      }
      return { height: "350px" };
    }
    return null;
  };

  handleSubmit = (event) => {
    event.preventDefault();
    if (!this.state.type) {
      this.setState({ error: "Discount type is required" });
      return;
    }
    if (this.state.type === "composite" && !this.state.operator) {
      this.setState({
        error: "Operator is needed with composite discount type",
      });
      return;
    }
    if (
      this.state.type &&
      this.state.type !== "composite" &&
      this.state.type !== "basket" &&
      !this.state.value
    ) {
      this.setState({ error: "Value is needed with selected policy type" });
      return;
    }
    if (!this.state.startTime || !this.state.endTime) {
      this.setState({ error: "Start date and end date are required" });
    }
    if (
      (this.state.type === "basket" ||
        this.state.type === "product" ||
        this.state.type === "category") &&
      !this.state.percentage
    ) {
      this.setState({
        error: "Discount percentage is required with given type",
      });
    }
    this.setState({ error: undefined });
    this.props.isRoot
      ? this.props
          .dispatch(
            addStoreRootDiscount(this.props.storeGuid, {
              type: this.state.type,
              operator: this.state.operator ? this.state.operator : undefined,
              startTime: this.state.startTime,
              endTime: this.state.endTime,
            })
          )
          .then((success) => {
            if (success) {
              this.props.closeDialog();
              this.props.dispatch(fetchStoreDiscounts(this.props.storeGuid));
            }
          })
      : this.props
          .dispatch(
            addStoreSubDiscount(
              this.props.storeGuid,
              this.props.fatherDiscountGuid,
              {
                type: this.state.type,
                operator: this.state.operator ? this.state.operator : undefined,
                productGuid: this.state.productGuid
                  ? this.state.productGuid
                  : undefined,
                category: this.state.category ? this.state.category : undefined,
                percentage: this.state.percentage,
                startTime: this.state.startTime,
                endTime: this.state.endTime,
              }
            )
          )
          .then((success) => {
            if (success) {
              this.props.closeDialog();
              this.setState({
                error: undefined,
                type: "",
                productGuid: "",
                category: "",
                startTime: "",
                endTime: "",
                percentage: 0,
                isMenuOpen: false,
              });
              this.props.dispatch(fetchStoreDiscounts(this.props.storeGuid));
            }
          });
  };

  render() {
    return (
      <Dialog
        open={this.props.isDialogOpen}
        onClose={this.handleClose}
        aria-labelledby="form-dialog-title"
      >
        <DialogTitle id="form-dialog-title">Add product</DialogTitle>
        <form>
          <DialogContent style={this.dynamicStyle()}>
            <DialogContentText>
              Please fill out the following discount details:
            </DialogContentText>
            <label>Type:</label>
            <Select
              onMenuOpen={this.handleOpenMenu}
              onMenuClose={this.handleCloseMenu}
              options={
                this.props.isRoot
                  ? [
                      { value: "composite", label: "Composite", name: "type" },
                      { value: "binary", label: "Binary", name: "type" },
                    ]
                  : typeOptions
              }
              onChange={this.handleChange}
            />
            {this.state.type && this.state.type === "product" ? (
              <React.Fragment>
                <br />
                <label>Product:</label>
                <Select
                  maxMenuHeight={200}
                  onMenuOpen={this.handleOpenMenu}
                  onMenuClose={this.handleCloseMenu}
                  options={this.props?.storeProducts.map((product) => {
                    return {
                      value: product.guid,
                      label: product.name,
                      name: "productGuid",
                    };
                  })}
                  onChange={this.handleChange}
                />{" "}
              </React.Fragment>
            ) : null}
            {this.state.type && this.state.type === "category" ? (
              <React.Fragment>
                <br />
                <label>Category:</label>
                <Select
                  maxMenuHeight={200}
                  onMenuOpen={this.handleOpenMenu}
                  onMenuClose={this.handleCloseMenu}
                  options={[
                    ...new Set(
                      this.props.storeProducts?.map(
                        (product) => product.category
                      )
                    ),
                  ].map((category) => {
                    return {
                      value: category,
                      label: category,
                      name: "category",
                    };
                  })}
                  onChange={this.handleChange}
                />{" "}
              </React.Fragment>
            ) : null}
            {this.state.type === "composite" ? (
              <React.Fragment>
                <br />
                <label>Operator:</label>
                <Select
                  onMenuOpen={this.handleOpenMenu}
                  onMenuClose={this.handleCloseMenu}
                  options={operatorOptions}
                  onChange={this.handleChange}
                />{" "}
              </React.Fragment>
            ) : null}
            {this.state.type && this.state.type !== "composite" ? (
              <React.Fragment>
                <br />
                <label>Percentage:</label>
                <input
                  type="text"
                  className="form-control mb-2"
                  name="percentage"
                  required
                  value={this.state.percentage}
                  onChange={this.handleChange}
                ></input>
              </React.Fragment>
            ) : null}
            {this.state.type && this.state.type !== "composite" ? (
              <React.Fragment>
                <div>
                  <br />
                  <label>Start time:</label>
                  <DatePicker
                    selected={this.state.startTime}
                    name="startTime"
                    onChange={this.handleChange} //only when value has changed
                  />
                </div>
                <div>
                  <br />
                  <label>End time:</label>
                  <DatePicker
                    selected={this.state.endTime}
                    name="endTime"
                    onChange={this.handleChange} //only when value has changed
                  />
                </div>{" "}
              </React.Fragment>
            ) : null}
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
          {(this.props.error || this.state.error) && (
            <Alert severity="error" onClick={() => this.setState(null)}>
              {this.props.error || this.state.error}
            </Alert>
          )}
        </form>
      </Dialog>
    );
  }
}

const mapStateToProps = (store) => {
  return {
    storeProducts: store.store.products,
  };
};

export default connect(mapStateToProps)(AddStorePolicyDiscount);
