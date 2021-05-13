import React, { Component } from "react";
import { connect } from "react-redux";
import "../css/productFilter.css";
import { filterProducts } from "../actions/productsActions";

class ProductFilter extends Component {
  state = {
    minPrice: 1,
    maxPrice: 1,
    MinPriceFilter: 1,
    MaxPriceFilter: 1,
    searchFilter: "",
  };

  componentDidUpdate(prevProps) {
    if (prevProps !== this.props) {
      let productsPrices = this.props.products.map((product) => product.price);
      let filteredProductsPrices = this.props.filteredProducts.map(
        (product) => product.price
      );
      let minPrice = this.props.products.length
        ? Math.min(...productsPrices)
        : 1;
      let maxPrice = this.props.products.length
        ? Math.max(...productsPrices)
        : 1;
      let filteredMaxPrice = this.props.filteredProducts.length
        ? Math.max(...filteredProductsPrices)
        : 1;
      let filteredMinPrice = this.props.filteredProducts.length
        ? Math.min(...filteredProductsPrices)
        : 1;
      this.setState({
        minPrice,
        maxPrice,
        MaxPriceFilter: filteredMaxPrice,
        MinPriceFilter: filteredMinPrice,
      });
    }
  }

  handleSearchFilter = (event) => {
    this.clearFilters();
    this.setState({
      [event.target.name]: event.target.value,
    });
    let keyword = event.target.value;
    if (!keyword) {
      this.props.dispatch(filterProducts(this.props.products));
    } else {
      var filteredProducts = this.props.products.filter(
        (product) =>
          product.name.includes(keyword) || product.category.includes(keyword)
      );
      this.props.dispatch(filterProducts(filteredProducts));
    }
  };

  handleCategoryFilter = (event) => {
    this.clearFilters();
    let category = event.target.value;
    var filteredProducts = this.props.products.filter(
      (product) => product.category === category
    );
    this.props.dispatch(filterProducts(filteredProducts));
  };

  handlePriceFilter = (event) => {
    this.clearFilters();
    var filteredProducts = this.props.products.filter(
      (product) =>
        product.price >= this.state.MinPriceFilter &&
        product.price <= this.state.MaxPriceFilter
    );
    this.props.dispatch(filterProducts(filteredProducts));
  };

  handleInputDisable = (event) => {
    event.preventDefault();
  };

  handleChange = (event) => {
    this.setState({
      [event.target.name]: event.target.value,
    });
  };

  handleClear = (event) => {
    event.preventDefault();
    this.clearFilters();
  };

  clearFilters = () => {
    this.setState({
      MinPriceFilter: this.state.minPrice,
      MaxPriceFilter: this.state.maxPrice,
      searchFilter: "",
    });
    this.props.dispatch(filterProducts(this.props.products));
  };

  render() {
    return (
      <aside
        className="col-md-3"
        style={
          this.props.maxWidthStyle
            ? { maxWidth: this.props.maxWidthStyle }
            : null
        }
      >
        <div className="card">
          <article className="filter-group">
            <header className="card-header">
              <div className="row align-items-center">
                <h4 className="title col-lg-9">Product Filters</h4>
                <button
                  onClick={this.handleClear}
                  type="submit ml-2"
                  className="btn btn-outline-primary"
                >
                  Clear
                </button>
              </div>
            </header>
            <div className="card-body border-bottom">
              <h5 className="card-title">Search</h5>

              <div className="input-group">
                <input
                  type="text"
                  placeholder="Keyword"
                  className="form-control"
                  name="searchFilter"
                  value={this.state.searchFilter}
                  onChange={this.handleSearchFilter}
                ></input>
              </div>
            </div>
            <div className="card-body">
              <h5 className="card-title">Product category</h5>
              <ul className="list-menu">
                {this.props.categories
                  ?.filter((category, index) => index < 10)
                  .map((category, index) => (
                    <li key={index}>
                      <button
                        value={category}
                        onClick={this.handleCategoryFilter}
                        className="btn btn-outline-primary btn-block mt-2"
                      >
                        {" "}
                        {category}
                      </button>
                    </li>
                  ))}
              </ul>
            </div>
            <div className="card">
              <div className="card-body">
                <h5 className="card-title">Price range</h5>
                <h6>Min Price</h6>
                <input
                  type="range"
                  className="custom-range"
                  min={this.state.minPrice}
                  max={this.state.MaxPriceFilter}
                  value={this.state.MinPriceFilter}
                  onInput={this.handleChange}
                  onChange={this.handleChange}
                  name="MinPriceFilter"
                ></input>
                <div className="form-row">
                  <div className="form-group col-md-6">
                    <label>Min Price:</label>
                    <input
                      className="form-control"
                      onKeyDown={this.handleInputDisable}
                      min={this.state.minPrice}
                      max={this.state.MaxPriceFilter}
                      placeholder={this.state.minPrice}
                      value={this.state.MinPriceFilter}
                      onChange={this.handleChange}
                      name="MinPriceFilter"
                      type="number"
                    ></input>
                  </div>
                </div>
                <h6>Max Price</h6>
                <input
                  type="range"
                  className="custom-range"
                  min={this.state.MinPriceFilter}
                  max={this.state.maxPrice}
                  value={this.state.MaxPriceFilter}
                  onInput={this.handleChange}
                  onChange={this.handleChange}
                  name="MaxPriceFilter"
                ></input>
                <div className="form-row">
                  <div className="form-group col-md-6">
                    <label>Max Price:</label>
                    <input
                      className="form-control"
                      onKeyDown={this.handleInputDisable}
                      min={this.state.MinPriceFilter}
                      max={this.state.maxPrice}
                      placeholder={this.state.minPrice}
                      value={this.state.MaxPriceFilter}
                      onChange={this.handleChange}
                      name="MaxPriceFilter"
                      type="number"
                    ></input>
                  </div>
                </div>
                <button
                  onClick={this.handlePriceFilter}
                  className="btn btn-block btn-outline-primary"
                >
                  Apply
                </button>
              </div>
            </div>
          </article>
        </div>
      </aside>
    );
  }
}

export default connect()(ProductFilter);
