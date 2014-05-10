// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TemporarySqlLocalDbInstance.cs" company="http://sqllocaldb.codeplex.com">
//   Martin Costello (c) 2012-2014
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   TemporarySqlLocalDbInstance.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace System.Data.SqlLocalDb
{
    /// <summary>
    /// A class representing a temporary SQL LocalDB instance.
    /// </summary>
    /// <remarks>
    /// The temporary SQL LocalDB instances that are created by instances of this class are automatically
    /// started when they are instantiated, and are then subsequently deleted when they are disposed of.
    /// </remarks>
    public class TemporarySqlLocalDbInstance : IDisposable
    {
        #region Fields

        /// <summary>
        /// The default <see cref="ISqlLocalDbProvider"/> instance to use to create temporary instances.
        /// </summary>
        private static readonly ISqlLocalDbProvider DefaultProvider = new SqlLocalDbProvider();

        /// <summary>
        /// The temporary SQL LocalDB instance.
        /// </summary>
        private readonly ISqlLocalDbInstance _instance;

        /// <summary>
        /// Whether the instance has been disposed.
        /// </summary>
        private bool _disposed;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TemporarySqlLocalDbInstance"/> class.
        /// </summary>
        /// <param name="instanceName">The name of the temporary SQL LocalDB instance.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instanceName"/> is <see langword="null"/>.
        /// </exception>
        public TemporarySqlLocalDbInstance(string instanceName)
            : this(instanceName, DefaultProvider)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemporarySqlLocalDbInstance"/> class.
        /// </summary>
        /// <param name="instanceName">The name of the temporary SQL LocalDB instance.</param>
        /// <param name="provider">The <see cref="ISqlLocalDbProvider"/> to use to create the temporary instance.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instanceName"/> or <paramref name="provider"/> is <see langword="null"/>.
        /// </exception>
        public TemporarySqlLocalDbInstance(string instanceName, ISqlLocalDbProvider provider)
        {
            if (instanceName == null)
            {
                throw new ArgumentNullException("instanceName");
            }

            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            _instance = provider.CreateInstance(instanceName);

            try
            {
                _instance.Start();
            }
            catch (Exception)
            {
                SqlLocalDbInstance.Delete(_instance);
                throw;
            }
        }

        #endregion

        #region Finalizer

        /// <summary>
        /// Finalizes an instance of the <see cref="TemporarySqlLocalDbInstance"/> class.
        /// </summary>
        ~TemporarySqlLocalDbInstance()
        {
            Dispose(false);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the temporary SQL LocalDB instance associated with this instance.
        /// </summary>
        public ISqlLocalDbInstance Instance
        {
            get { return _instance; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new instance of <see cref="TemporarySqlLocalDbInstance"/> with a randomly assigned name.
        /// </summary>
        /// <returns>
        /// The created instance of <see cref="TemporarySqlLocalDbInstance"/>.
        /// </returns>
        public static TemporarySqlLocalDbInstance Create()
        {
            string instanceName = Guid.NewGuid().ToString();
            return new TemporarySqlLocalDbInstance(instanceName);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">
        /// <see langword="true" /> to release both managed and unmanaged resources;
        /// <see langword="false" /> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (_instance != null)
                {
                    _instance.Stop();
                    SqlLocalDbInstance.Delete(_instance);
                }

                _disposed = true;
            }
        }

        #endregion
    }
}